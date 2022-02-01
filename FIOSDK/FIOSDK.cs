using Godot;
using System;
using System.Collections.Generic;

public class FIOSDK : Node
{
	String privateKey;
	String publicKey;
	String registerMockUrl;
	String technologyProviderId;
	bool returnPreparedTx;

	public override void _Ready()
	{
		
	}

	public void Init(String privateKey, String publicKey, String baseUrl, String registerMockUrl, String tecknologyProviderId, bool returnPreparedTx) 
	{
		this.privateKey = privateKey;
		this.publicKey = publicKey;
		

		GD.Print(privateKey);
		GD.Print(publicKey);
		GD.Print(baseUrl);
	}

	

  
}
