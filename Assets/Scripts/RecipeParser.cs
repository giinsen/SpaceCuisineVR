using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeParser : MonoBehaviour 
{
	public static RecipeParser instance;
	public OrderList[] orderLists;

	private void Awake()
	{
		instance = this;

		string csv = File.ReadAllText(Application.dataPath + "/orders_list.csv");
		orderLists = ParseCSV(csv);
	}

	public OrderList[] ParseCSV(string csv)
	{
		OrderList[] result = {new OrderList(), new OrderList(), new OrderList()};

		string[] lines = csv.Split("\n"[0]);
		foreach (string str in lines)
		{
			string[] columns = str.Split(';');
			if (columns.Count() == 2)
			{
				result[int.Parse(columns[0])].recipes.Add(columns[1]);
			}
		}
		return result;
	}
}

public class OrderList
{
	public List<string> recipes  = new List<string>();

	private int idx = -1;

	public OrderList()
	{
		this.recipes = new List<string>();
		this.idx = -1;
	}

	public OrderList(List<string> recipes)
	{
		this.recipes = recipes;
		this.idx = -1;
	}

	public bool IsOver()
	{
		return idx < recipes.Count;
	}

	public string GetNext()
	{
		idx++;
		if (IsOver())
		{
			return recipes[recipes.Count];
		}
		return recipes[idx];
	}
}