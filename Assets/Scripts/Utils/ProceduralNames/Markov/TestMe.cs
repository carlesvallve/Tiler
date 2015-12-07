using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Markov;

public class TestMe : MonoBehaviour {

	void Awake() {
		GenerateRandomNames();
	}


	void GenerateRandomNames () {
		Text text = GameObject.Find("Text").GetComponent<Text>();
		text.text = "";

		string path = "Assets/Scripts/Utils/ProceduralNames/NameFiles/";

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
		string str =@"Dear Riot,
My name is Carles Vallve. I was born in Barcelona Spain in 1972, and my life has always been involved around games and interactivity.
Probably everything started when I got myself my first Sinclair ZX Spectrum 16k when I discovered that I didn't only like to play those games after typing the Load command, but what really excited me beyond limits was to create my own routines and see those binary rows and columns of ones and zeros come to life afterwards in the small black and white TV screen.
A lot has changed since those days, but the same principle has accompanied me during all this time.
To make small entities come to life, to suggest intelligence -and intentionality- to the senses of the observer, to guide someone through an amazing story, leaving his options as open or close as beauty and narrative demands, or even better, to let someone imagine and feel his own story, as he submerges into a realm of bits that he can easily convert into his own desires.
Technology changes quickly, and what we know today is transformed into what we apply tomorrow faster than we usually realize, so at the end, a seasoned genuine passion is what can make a difference.
Having established myself in Tokyo for the last 8 years, I have been spending a quite big amount of time working on casual gaming on mobile platforms, both in Unity/C# and Javascript/WebGL, either as freelancer and in-house development with international groups of people, using agile processes and taking games from early stages into production. But after all this time, I feel like the moment has come to be a bit more ambitious and try my luck working in bigger games that have more depth and scope, and are generally more interesting.
This is why I am writing to you today. After having put an eye out to see what's out there, I read your job offer at Riot Games website, and I suddenly felt that you guys where just talking about me. I look forward to having an opportunity to speak to you more about position. 
Yours sincerely, Carles Vallve";
		

		MarkovChainGenerator mc = new MarkovChainGenerator();
		mc.Load(str);

		List<string> items = new List<string>();

		// This was added to a button in form1 file...
		/*Markov.Structs.RootWord w = new Markov.Structs.RootWord();
		foreach(object x in mc.Words)
		{
			w=(Markov.Structs.RootWord)mc.Words[((System.Collections.DictionaryEntry)x).Key];
			items.Add(w.Word);
			//print(w.Word);
		}*/

		// This was added to a button in form1 file...
		/*private void lBRoot_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string index=(string)lBRoot.Items[lBRoot.SelectedIndex];
			index=index.ToLower();
			Markov.Structs.RootWord w = new Markov.Structs.RootWord();
			Markov.Structs.Child	 c = new Markov.Structs.Child();
			w=(Markov.Structs.RootWord)mc.Words[index];
			lBChild.Items.Clear();
			foreach(object x in w.Childs)
			{
				c=(Markov.Structs.Child)w.Childs[((System.Collections.DictionaryEntry)x).Key];
				lBChild.Items.Add(c.Word+"-"+( 
												 ( (double)c.Occurrence/(double)w.ChildCount )
												 *
												 100
											  )+"%"
								 );
			}
		}*/

		string output = mc.Output();
		GameObject.Find("Text").GetComponent<Text>().text = mc.Words.Count + "\n" + output;

	}
}
