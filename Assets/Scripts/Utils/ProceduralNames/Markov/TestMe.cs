using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Markov;

public class TestMe : MonoBehaviour {

	void Start() {
		//GenerateRandomNames();
		//GenerateRandomText();
		//Text text = GameObject.Find("Text").GetComponent<Text>();
		//text.text = CsvReader.Load("Data/GameData/Monsters");
	}


	void GenerateRandomNames () {
		Text text = GameObject.Find("Text").GetComponent<Text>();
		text.text = "";

		string path = "Assets/Scripts/Utils/ProceduralNames/Data/Names/";

		ProceduralNameGenerator maleNames = new ProceduralNameGenerator(path + "Male.txt");
		ProceduralNameGenerator femaleNames = new ProceduralNameGenerator(path + "Female.txt");
		ProceduralNameGenerator ukranianNames = new ProceduralNameGenerator(path + "Ukranian.txt");
        
        int max = 8;

        for (int i = 0; i < max; i++) {
            string word = maleNames.GenerateRandomWord(Random.Range(3, 7));
            text.text += word + "\n";
        }

        text.text += "\n";

        for (int i = 0; i < max; i++) {
            string word = femaleNames.GenerateRandomWord(Random.Range(3, 7));
            text.text += word + "\n";
        }

         text.text += "\n";

        for (int i = 0; i < max; i++) {
            string word = ukranianNames.GenerateRandomWord(Random.Range(3, 7));
            text.text += word + "\n";
        }
	}


	void GenerateRandomText () {

		string path = "Assets/Scripts/Utils/ProceduralNames/Data/Story/";

		MarkovChainGenerator mc = new MarkovChainGenerator();
		mc.Load(path + "IrishTale.txt");
		string output = mc.Output();

		GameObject.Find("Text").GetComponent<Text>().text = output.Length + "/" + mc.Words.Count + "\n" + output;
		Debug.Log (output.Length + " " + output);

	}
}
