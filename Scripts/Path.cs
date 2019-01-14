using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {
	public Color lineColor;																			//Public line color variable
	public List<Transform> nodes=new List<Transform>();											//Creates a list instead of an array to store our path nodes
																									//List is accessible through System.Collections.Generic
		
	void OnDrawGizmos(){																	//Draws all "Gizmos" when the element is selected
		Gizmos.color=lineColor; 																	//Sets the color of the gizmos
		
		Transform[] pathTransforms=GetComponentsInChildren<Transform>();							//Stores all the Transform children from Path into one array
		
		nodes=new List<Transform>();
		for(int i=0; i<pathTransforms.Length; i++){													//Cycles through the entire array of pathTransforms
			if(pathTransforms[i]!=transform){														//If the current transform is not the parent node...
				nodes.Add(pathTransforms[i]);														//Add current transform to the nodes list
			}
		}
		
		for(int i=0; i<nodes.Count; i++){
			Vector3 currentNode=nodes[i].position;													//Takes the pos of the current node and stores it in currentNode
			Vector3 previousNode = Vector3.zero;
			if(i>0){																				//Checks if the index isn't the first one in the array
				previousNode=nodes[i-1].position;													//Takes the pos of the previous node and stores it in previousNode
			}else if(i==0 && nodes.Count>1){														//Checks if the index is the first one in the array
				previousNode=nodes[nodes.Count-1].position;											//Takes the pos of the LAST node in the array and stores it in previousNode
			}
			
			Gizmos.DrawLine(previousNode, currentNode);												//Draws a line from the previous to current node
			Gizmos.DrawWireSphere(currentNode, 0.3f);												//Draws a sphere on the current node
			
		}
	}
}