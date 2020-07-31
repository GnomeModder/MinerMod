using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MinerSkins : MonoBehaviour
{

	public Material[] materials;
	public Mesh[] meshes;
	SkinnedMeshRenderer renderer;
	public int index = 0;
	int indexlog;

	void Start()
	{
		renderer = GetComponent<SkinnedMeshRenderer>();
		renderer.sharedMaterial = materials[0];
		renderer.sharedMesh = meshes[0];
		indexlog = 0;
	}

	// Update is called once per frame
	void Update()
	{ 
		if (index != indexlog && index >= 0 && index < 3)
		{
			renderer.sharedMaterial = materials[index];
			renderer.sharedMesh = meshes[index];
			indexlog = index;
		}
	}
}
