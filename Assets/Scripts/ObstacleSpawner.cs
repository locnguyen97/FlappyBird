using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

	[SerializeField] private float waitTime;
	[SerializeField] private GameObject[] obstaclePrefabs;
	[SerializeField] private GameObject pipePrefabs;

	[SerializeField] private float maxposy;
	[SerializeField] private float minposy;
	[SerializeField] private float harrdDistance;
	[SerializeField] private float easyDistance;

	private int turnSpawn = 0;
	private float tempTime;
	private float tempPosY;
	private float posY;
	private Vector3 posSpawn;

	private bool fistTime = false;

	void Start(){
		tempTime = waitTime - Time.deltaTime;
	}

	void LateUpdate () {
		if(GameManager.Instance.GameState()){
			tempTime += Time.deltaTime;
			if(tempTime > waitTime){
				// Wait for some time, create an obstacle, then set wait time to 0 and start again
				tempTime = 0;
				//GameObject pipeClone = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], transform.position, transform.rotation);
				posSpawn = transform.position;
				if (fistTime == false)
				{
					tempPosY = Random.Range(minposy, maxposy);
					fistTime = true;
				}
				else
				{
					if (turnSpawn < 3) //easy
					{
						int so = Random.Range(0, 2);
						if (so == 0)
						{
							if (tempPosY + easyDistance < maxposy)
							{
								tempPosY += easyDistance;
							}
							else
							{
								tempPosY -= easyDistance;
							}
						}
						else
						{
							if (tempPosY - easyDistance > minposy)
							{
								tempPosY -= easyDistance;
							}
							else
							{
								tempPosY += easyDistance;
							}
						}
					}
					else // hard
					{
						int so = Random.Range(0, 2);
						if (so == 0)
						{
							if (tempPosY + harrdDistance < maxposy)
							{
								tempPosY += harrdDistance;
							}
							else
							{
								tempPosY -= harrdDistance;
							}
						}
						else
						{
							if (tempPosY - harrdDistance > minposy)
							{
								tempPosY -= harrdDistance;
							}
							else
							{
								tempPosY += harrdDistance;
							}
						}
					}
				}

				posSpawn.y = tempPosY;
				GameObject pipeClone = Instantiate(pipePrefabs, posSpawn, transform.rotation);

				fistTime = true;
				turnSpawn++;
				if (turnSpawn == 5) turnSpawn = 0;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.transform.parent != null){
			Destroy(col.gameObject.transform.parent.gameObject);
		}else{
			Destroy(col.gameObject);
		}
	}

}
