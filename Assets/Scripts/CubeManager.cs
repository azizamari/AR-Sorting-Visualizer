﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class CubeManager : MonoBehaviour
{
    public int numberOfCubes;
    public int cubeHeightMax = GameData.cubeHeightMax;
    public GameObject cube;
    //public GameObject Generator;
    //public GameObject Generator2;
    public GameObject sortButton;
    public List<BarGraph> barGraphs= new List<BarGraph>();
    public List<int> deletedBars = new List<int>();
    private void Start()
    {
        numberOfCubes = GameData.numberOfCubes;
        cubeHeightMax = GameData.cubeHeightMax;
        //barGraphs.Add(new BarGraph(Generator, numberOfCubes));
        //barGraphs.Add(new BarGraph(Generator2, numberOfCubes, SortType.Bubble));
        Time.timeScale = 3;
    }
    public void AddIndexToDeletedBarsList(int index)
    {
        deletedBars.Add(index);
    }
    public void GenerateBlocks()
    {
        sortButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        StopAllCoroutines();
        resetBarGraphsCubesArray(barGraphs);
        for(int x = 0;x<barGraphs.Count;x++)
        {
            if (!deletedBars.Contains(x))
            {
                BarGraph graph = barGraphs[x];
                if (!graph.exists)
                {
                    for (int i = 0; i < numberOfCubes; i++)
                    {
                        int randomHeight = Random.Range(1, cubeHeightMax + 1);
                        GameObject instance = Instantiate(cube, graph.position.transform.position, graph.position.transform.localRotation);
                        instance.transform.position = new Vector3(graph.position.transform.position.x + i * instance.transform.localScale.x, graph.position.transform.position.y + ((float)randomHeight / 50 / 2.0f), graph.position.transform.position.z);
                        instance.transform.localScale = new Vector3(instance.transform.localScale.x * 0.8f, (float)randomHeight / 50, instance.transform.localScale.z);
                        instance.transform.parent = graph.position.transform;

                        graph.cubesArray[i] = instance;
                        graph.exists = true;
                    }
                }
                else
                {
                    graph.position.GetComponent<Index>().index = x;
                    for (int i = 0; i < numberOfCubes; i++)
                    {
                        graph.cubesArray[i].transform.GetComponent<MeshRenderer>().material.color = cube.transform.GetComponent<MeshRenderer>().material.color;
                        int randomHeight = Random.Range(1, cubeHeightMax + 1);
                        graph.cubesArray[i].transform.position = new Vector3(graph.cubesArray[i].transform.position.x, graph.position.transform.position.y + ((float)randomHeight / 50 / 2.0f), graph.cubesArray[i].transform.position.z);
                        graph.cubesArray[i].transform.localScale = new Vector3(graph.cubesArray[i].transform.localScale.x * 0.8f, (float)randomHeight / 50, graph.cubesArray[i].transform.localScale.z);
                    }
                }
            }
        }
    }
    public void StartSort()
    {
        sortButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        foreach(var bar in barGraphs)
        {
            if (bar.sortType == SortType.Selection)
            {
                StartCoroutine(SelectionSort(bar));
            }
            else if (bar.sortType == SortType.Bubble)
            {
                StartCoroutine(BubbleSort(bar));
            }
            else if (bar.sortType == SortType.Merge)
            {
                StartCoroutine(ShellSort(bar));
            }
        }
    }
    IEnumerator BubbleSort(BarGraph graph)
    {
        Vector3 tempScale;
        for (int x = 0; x < numberOfCubes; x++)
        {

            for (int y = 0; y < numberOfCubes - x - 1; y++)
            {
                if (graph.cubesArray[y].transform.localScale.y > graph.cubesArray[y + 1].transform.localScale.y)
                {
                    yield return new WaitForSeconds(1f);
                    //temp = graph.cubesArray[y];
                    //graph.cubesArray[y] = graph.cubesArray[y + 1];
                    //graph.cubesArray[y + 1] = temp;

                    graph.cubesArray[y].transform.position = new Vector3(graph.cubesArray[y].transform.position.x, graph.position.transform.position.y + graph.cubesArray[y+1].transform.localScale.y / 2, graph.cubesArray[y].transform.position.z);
                    graph.cubesArray[y+1].transform.position = new Vector3(graph.cubesArray[y+1].transform.position.x, graph.position.transform.position.y + graph.cubesArray[y].transform.localScale.y / 2, graph.cubesArray[y+1].transform.position.z);
                    tempScale = graph.cubesArray[y].transform.localScale;
                    graph.cubesArray[y].transform.localScale = graph.cubesArray[y+1].transform.localScale;
                    graph.cubesArray[y+1].transform.localScale = tempScale;
                    //unsortedList[y].transform.localPosition = new Vector3(unsortedList[y + 1].transform.localPosition.x, tempPos.y, tempPos.z);
                    //unsortedList[y + 1].transform.localPosition = new Vector3(tempPos.x, unsortedList[y + 1].transform.localPosition.y, unsortedList[y + 1].transform.localPosition.z);
                    LeanTween.color(graph.cubesArray[y], Color.red, .3f);
                    LeanTween.color(graph.cubesArray[y+1], Color.red, .3f);
                    LeanTween.color(graph.cubesArray[y+1], graph.cubesArray[y].transform.GetComponent<MeshRenderer>().material.color, .1f).setDelay(.6f);
                    LeanTween.color(graph.cubesArray[y], graph.cubesArray[y].transform.GetComponent<MeshRenderer>().material.color, .1f).setDelay(.6f);

                    //LeanTween.moveX(graph.cubesArray[y], graph.cubesArray[y+1].transform.position.x, 1f);
                    //LeanTween.moveZ(graph.cubesArray[y], .03f, .5f).setLoopPingPong(1);

                    //LeanTween.moveX(graph.cubesArray[y+1], graph.cubesArray[y].transform.position.x, 1f);
                    //LeanTween.moveZ(graph.cubesArray[y+1], .03f, .5f).setLoopPingPong(1);
                }
            }
            LeanTween.color(graph.cubesArray[numberOfCubes-x-1], Color.green, .4f).setDelay(1f);

        }
    }
    IEnumerator SelectionSort(BarGraph graph)
    {
        int min;
        Vector3 tempScale;
        for(int i=0; i < graph.cubesArray.Length; i++)
        {
            yield return new WaitForSeconds(1f);
            min = i;
            for(int j = i; j < graph.cubesArray.Length; j++)
            {
                graph.cubesArray[j].transform.GetComponent<MeshRenderer>().material.color = Color.blue;
                yield return new WaitForSeconds(.2f);
                graph.cubesArray[j].transform.GetComponent<MeshRenderer>().material.color = new Color32(103, 207, 228,1);
                if (graph.cubesArray[j].transform.localScale.y < graph.cubesArray[min].transform.localScale.y)
                {
                    min = j;
                }
            }
            if (min != i) {
                yield return new WaitForSeconds(1.2f);
                //temp = graph.cubesArray[i];
                //graph.cubesArray[i] = graph.cubesArray[min];
                //graph.cubesArray[min] = temp;

                graph.cubesArray[i].transform.position = new Vector3(graph.cubesArray[i].transform.position.x,  graph.position.transform.position.y + graph.cubesArray[min].transform.localScale.y/2, graph.cubesArray[i].transform.position.z);
                graph.cubesArray[min].transform.position = new Vector3(graph.cubesArray[min].transform.position.x,graph.position.transform.position.y + graph.cubesArray[i].transform.localScale.y/2, graph.cubesArray[min].transform.position.z);
                tempScale = graph.cubesArray[i].transform.localScale;
                graph.cubesArray[i].transform.localScale = graph.cubesArray[min].transform.localScale;
                graph.cubesArray[min].transform.localScale = tempScale;

                //tempPos = unsortedList[i].transform.position;
                //unsortedList[i].transform.position = new Vector3(unsortedList[min].transform.position.x, tempPos.y, tempPos.z);
                //unsortedList[min].transform.position = new Vector3(tempPos.x, unsortedList[min].transform.position.y, unsortedList[min].transform.position.z);
                LeanTween.color(graph.cubesArray[i], Color.red, .4f);
                LeanTween.color(graph.cubesArray[min], Color.red, .4f);
                LeanTween.color(graph.cubesArray[min], graph.cubesArray[i].transform.GetComponent<MeshRenderer>().material.color, .1f).setDelay(.9f);

                //LeanTween.moveX(unsortedList[i], unsortedList[min].transform.position.x, 1f);
                //LeanTween.moveZ(unsortedList[i], .03f, .5f).setLoopPingPong(1);

                //LeanTween.moveX(unsortedList[min], unsortedList[i].transform.position.x, 1f);
                //LeanTween.moveZ(unsortedList[min], .03f, .5f).setLoopPingPong(1);
            }
            LeanTween.color(graph.cubesArray[i], Color.green, .1f).setDelay(.5f);
        }
    }
    IEnumerator ShellSort(BarGraph graph)
    {

        int n = graph.cubesArray.Length;
        int gap = n / 2;
        Vector3 temp;

        while (gap > 0)
        {
            for (int i = 0; i + gap < n; i++)
            {
                int j = i + gap;
                temp = graph.cubesArray[j].transform.localScale;

                while (j - gap >= 0 && temp.y < graph.cubesArray[j - gap].transform.localScale.y)
                {
                    yield return new WaitForSeconds(1.5f);

                    LeanTween.color(graph.cubesArray[j], Color.red, .3f);
                    LeanTween.color(graph.cubesArray[j], cube.transform.GetComponent<MeshRenderer>().material.color, .1f).setDelay(.7f);

                    graph.cubesArray[j].transform.position = new Vector3(graph.cubesArray[j].transform.position.x, graph.position.transform.position.y + graph.cubesArray[j-gap].transform.localScale.y / 2, graph.cubesArray[j].transform.position.z);
                    graph.cubesArray[j].transform.localScale = graph.cubesArray[j - gap].transform.localScale;
                    //array[j] = array[j - gap];

                    j = j - gap;
                }

                LeanTween.color(graph.cubesArray[j], Color.red, .3f);
                LeanTween.color(graph.cubesArray[j], cube.transform.GetComponent<MeshRenderer>().material.color, .1f).setDelay(.7f);

                graph.cubesArray[j].transform.position = new Vector3(graph.cubesArray[j].transform.position.x, graph.position.transform.position.y + temp.y / 2, graph.cubesArray[j].transform.position.z);
                graph.cubesArray[j].transform.localScale = temp;
                //array[j] = temp;
            }

            gap = gap / 2;
        }
        foreach(var cube in graph.cubesArray)
        {
            yield return new WaitForSeconds(.2f);
            LeanTween.color(cube, Color.green, .2f);
        }
    }
    public void resetBarGraphsCubesArray(List<BarGraph> list)
    {
        for(int x =0; x<list.Count;x++)
        {
            BarGraph graph = list[x];
            if (graph.exists&&!deletedBars.Contains(x))
            {
                resetArray(graph.cubesArray);
            }
        }
    }
    private void resetArray(GameObject[] cubes)
    {
        if (cubes != null)
        {
            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i].transform.localScale = cube.transform.localScale;
            }
            cubes = null;
        }
    }
    public void ChangeSortType(int i, SortType type)
    {
        if (barGraphs[i].sortType != type)
        {
            barGraphs[i].sortType = type;
            GenerateBlocks();
            GenerateBlocks();
        }
        else
            barGraphs[i].sortType = type;

    }
}
public enum SortType
{
    Selection,
    Bubble,
    Merge,
    Quick,
}
public class BarGraph
{
    public bool exists = false;
    public GameObject position;
    public SortType sortType = SortType.Selection;
    public GameObject[] cubesArray;
    int numberOfCubes =GameData.numberOfCubes;
    public BarGraph(GameObject position, int numberOfCubes, SortType sortType = SortType.Selection)
    {
        this.position = position;
        this.sortType = sortType;
        this.cubesArray = new GameObject[numberOfCubes];
    }
}