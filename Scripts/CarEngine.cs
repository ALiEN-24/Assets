using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarEngine : MonoBehaviour {
	[Header("[Wheels]")]
	public Transform path;																			//Holds a path variable
	public float maxSteerAngle=45f;																	//The max degrees the wheels can turn
	public float turnSpeed=3f;																		//Speed of which the car turns
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelRL;
	public WheelCollider wheelRR;
	public float maxMotorTorque=80f;																//The max amount of torque the wheels can have
	public float maxBrakeTorque=150f;
	public float currentSpeed;
	public float maxSpeed=100f;																		//The max speed the car can have
	public Vector3 centerOfMass;																	//Coords for the center of mass for the car
	public bool isBraking=false;																	//Checks if the car is braking
	public Texture2D textureNormal;																	//Textures for the car on normal conditions
	public Texture2D textureBraking;																//Textures for the car when braking
	public Renderer carRenderer;																	//The object to be rendering the textures
	
	[Header("[Sensors]")]
	public float sensorLength=3f;																	//Length of the car sensors
	public Vector3 frontSensorPos=new Vector3(0f,0.2f,0.5f);										//Pos of the front sensor
	public float frontSideSensorPos=0.2f;															//Pos of the side sensors
	public float frontSensorAngle=30f;
	
	public List<Transform> nodes;
	public int currentNode=0;
	public List<Transform> triggers;
	public int currentTrigger=0;
	private bool avoiding=false;																	//Checks if car is avoiding an obstacle
	private float targetSteerAngle=0f;																//Used to smooth the wheel angle instead of snapping
	
	void Start(){
		GetComponent<Rigidbody>().centerOfMass=centerOfMass;
		
		Transform[] pathTransforms=path.GetComponentsInChildren<Transform>();						//Stores all the Transform children from Path into one array
		
		nodes=new List<Transform>();
		for(int i=0; i<pathTransforms.Length; i++){													//Cycles through the entire array of pathTransforms
			if(pathTransforms[i]!=path.transform){													//If the current transform is not the parent node...
				nodes.Add(pathTransforms[i]);														//Add current transform to the nodes list
			}
		}
	}
	
	void FixedUpdate(){
		//Sensors();																					//Calls the function Sensors
		ApplySteer();																				//Calls the function ApplySteer
		Drive();																					//Calls the function Drive
		CheckWaypointDistance();																	//Calls the function CheckWaypointDistance
		Braking();																					//Calls the function Braking
		LerpToSteerAngle();																			//Calls the function lerpTosteerAngle()
		Triggers();
	}
	
	private void Sensors(){																			//Projects raycast lines of length sensorLength to detect obstacles
		RaycastHit hit;																				//Stores raycast information
		Vector3 sensorStartPos=transform.position;													//Starting pos of the sensor
		sensorStartPos+=transform.forward*frontSensorPos.z;											//Places sensor at the front of the car
		sensorStartPos+=transform.up*frontSensorPos.y;												//Places sensor higher, near the center of the car
		float avoidMultiplier=0f;																	//Multiplier of how much the wheels turn when avoiding obstacles
		avoiding=false;
		
		//Front-Right Sensor
		sensorStartPos+=transform.right*frontSideSensorPos; 
		if(Physics.Raycast(sensorStartPos,transform.forward,out hit, sensorLength)){				
			if(!hit.collider.CompareTag("Terrain")){												//Checks if the raycast is not hitting the terrain						
				Debug.DrawLine(sensorStartPos,hit.point);
				avoiding=true;
				avoidMultiplier-=1f;
			}
		}
		
		//Front-Right Angled Sensor
		else if(Physics.Raycast(sensorStartPos,Quaternion.AngleAxis(frontSensorAngle,transform.up)*transform.forward,out hit, sensorLength)){				
			if(!hit.collider.CompareTag("Terrain")){													
				Debug.DrawLine(sensorStartPos,hit.point);
				avoiding=true;
				avoidMultiplier-=0.5f;
			}
		}
		
		//Front-Left Sensor
		sensorStartPos-=transform.right*frontSideSensorPos*2; 
		if(Physics.Raycast(sensorStartPos,transform.forward,out hit, sensorLength)){				
			if(!hit.collider.CompareTag("Terrain")){													
				Debug.DrawLine(sensorStartPos,hit.point);
				avoiding=true;
				avoidMultiplier+=1f;
			}
		}
		
		//Front-Left Angled Sensor
		else if(Physics.Raycast(sensorStartPos,Quaternion.AngleAxis(-frontSensorAngle,transform.up)*transform.forward,out hit, sensorLength)){				
			if(!hit.collider.CompareTag("Terrain")){													
				Debug.DrawLine(sensorStartPos,hit.point);
				avoiding=true;
				avoidMultiplier+=0.5f;
			}
		}
		
		//Front-Center Sensor
		if(avoidMultiplier==0){
			if(Physics.Raycast(sensorStartPos,transform.forward,out hit, sensorLength)){
				if(!hit.collider.CompareTag("Terrain")){												
					avoiding=true;
					if(hit.normal.x<0){
						avoidMultiplier=-1;
					}else{
						avoidMultiplier=1;
					}
				}
			}
		}

		
		if(avoiding){
			targetSteerAngle=maxSteerAngle*avoidMultiplier;
		}
	}
	
	private void ApplySteer(){																		//Turns the front wheels towards the next node
		if(avoiding) return;																		//Exits ApplySteer if car is avoiding an obstacle
		Vector3 relativeVector=transform.InverseTransformPoint(nodes[currentNode].position);		//Calculates the vector between the current node and the car object
		float newSteer=relativeVector.x/relativeVector.magnitude;									//Simplifies the x vector to be -1 (left), 0 (straight), or 1 (right)
		newSteer=newSteer*maxSteerAngle;															//Calculates the degrees the wheels have to turn to get to the next node
		targetSteerAngle=newSteer;
	}
	
	private void Drive(){																			//Applies torque to the front wheels to make the car move
		currentSpeed=2*Mathf.PI*wheelFL.radius*wheelFL.rpm*60/1000;									//Calculates current speed based on how fast the wheel is spinning
		if(currentSpeed<maxSpeed && !isBraking){
			wheelFL.motorTorque=maxMotorTorque;
			wheelFR.motorTorque=maxMotorTorque;
		}else{
			wheelFL.motorTorque=0f;
			wheelFR.motorTorque=0f;
		}
	}
	
	private void CheckWaypointDistance(){															//Updates the node the car follows after a certain distance
		if(Vector3.Distance(transform.position, nodes[currentNode].position)<0.5f){					//If the distance between the car and the node is less than 0.5...
			if(currentNode==nodes.Count-1){															//...and if the current node is the last node...
				currentNode=0;																		//...then switch the current node the next node in line, 0
			}else{
				currentNode++;																		//...then switch current node to the next node
			}
		}
	}
	
	private void Braking(){																			//Controls whether or not the car is braking
		if(isBraking){
			carRenderer.material.mainTexture=textureBraking;										//Changes textures to the braking texture
			wheelRL.brakeTorque=maxBrakeTorque;
			wheelRR.brakeTorque=maxBrakeTorque;
		}else{
			carRenderer.material.mainTexture=textureNormal;											//Changes textures to the normal texture
			wheelRL.brakeTorque=0f;
			wheelRR.brakeTorque=0f;
		}
	}
	
	private void LerpToSteerAngle(){																//lerps the wheel angles so that turning corners is smoothed
		wheelFL.steerAngle=Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime*turnSpeed);
		wheelFR.steerAngle=Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime*turnSpeed);
	}
	
	private void Triggers(){
		
	}
}
