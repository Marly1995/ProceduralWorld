﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator 
{
	public static float[,] GenerateFalloff(int size, float a, float b)
	{
		float[,] falloff = new float[size,size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size * 2 - 1;
				float y = j / (float)size * 2 - 1;

				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				falloff[i, j] = EvaluateFalloff(value, a, b);
			}
		}
		return falloff;
	}
	static float EvaluateFalloff(float value, float a, float b)
	{
		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
	}
}
