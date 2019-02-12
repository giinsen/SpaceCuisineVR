using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour 
{
	public static int difficultyScore = 15;

	public RecipeTrack[] tracks;

	public static void RegisterSuccess()
	{
		difficultyScore += 5;
		if (difficultyScore > 100)
			difficultyScore = 100;
	}

	public static void RegisterFailure()
	{
		difficultyScore -=2;
		if (difficultyScore < 1)
			difficultyScore = 1;
	}

	private void Start()
	{
		StartCoroutine(Process());
	}

	private IEnumerator Process()
	{
		while (true)
		{
			yield return new WaitForSeconds(30.0f);
			if (AnyTrackFree())
			{
				GetRandomTrack().RequestOrder();
			}
			else
			{
				while (AnyTrackFree() == false)
				{
					yield return new WaitForEndOfFrame();
				}
			}


		}
	}

	private bool AnyTrackFree()
	{
		foreach(RecipeTrack track in tracks)
		{
			if (track.waitForOrder == false)
				return true;
		}
		return false;
	}

	private RecipeTrack GetRandomTrack()
	{
		List<RecipeTrack> available = new List<RecipeTrack>();
		foreach(RecipeTrack track in tracks)
		{
			if (track.waitForOrder == false)
				available.Add(track);
		}
		return available[Random.Range(0, available.Count)];
	}
}
