using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerParent : MonoBehaviour {
	public List<Transform> triggers=new List<Transform>();
	
	void Start(){
		Transform[] triggerTransforms=GetComponentsInChildren<Transform>();							//Stores all the Transform children from Path into one array
		
		triggers=new List<Transform>();
		for(int i=0; i<triggerTransforms.Length; i++){												//Cycles through the entire array of triggerTransforms
			if(triggerTransforms[i]!=transform){													//If the current transform is not the parent node...
				triggers.Add(triggerTransforms[i]);													//Add current transform to the triggers list
			}
		}
		
		for(int i=0; i<triggers.Count; i++){
			Vector3 currentTrigger=triggers[i].position;												//Takes the pos of the current node and stores it in currentTrigger
			Vector3 previousTrigger = Vector3.zero;
			if(i>0){																				//Checks if the index isn't the first one in the array
				previousTrigger=triggers[i-1].position;												//Takes the pos of the previous node and stores it in previousTrigger
			}else if(i==0 && triggers.Count>1){														//Checks if the index is the first one in the array
				previousTrigger=triggers[triggers.Count-1].position;									//Takes the pos of the LAST node in the array and stores it in previousTrigger
			}
		}
	}
	
	void Update(){
		
	}
}
