using UnityEngine;
using System;
using System.Collections.Generic;


//Generates random names based on the statistical weight of letter sequences
//in a collection of sample names
public class MarkovNameGenerator
{
	#region Names
	public static readonly string[] RussianMaleNames =
	{
		"Аввакум",
		"Август",
		"Авдий",
		"Аверьян",
		"Авксентий",
		"Авраам",
		"Агап",
		"Агафон",
		"Агей",
		"Адам",
		"Адриан",
		"Азарий",
		"Аким",
		"Алан",
		"Александр",
		"Алексей",
		"Альберт",
		"Альфред",
		"Амброзий",
		"Ананий",
		"Анастас",
		"Анатолий",
		"Андрей",
		"Андрон",
		"Анисим",
		"Антип",
		"Антон",
		"Ануфрий",
		"Анфим",
		"Аполлон",
		"Аристарх",
		"Аркадий",
		"Арсений",
		"Артамон",
		"Артем",
		"Артур",
		"Архип",
		"Аскольд",
		"Афанасий",
		"Афиноген",
		"Африкан",
		"Бажен",
		"Бенедикт",
		"Богдан",
		"Боримир",
		"Борис",
		"Боян",
		"Брачислав",
		"Будимир",
		"Булат",
		"Вавила",
		"Вадим",
		"Валентин",
		"Валериан",
		"Валерий",
		"Варлаам",
		"Варфоломей",
		"Василий",
		"Вениамин",
		"Викентий",
		"Виктор",
		"Вильям",
		"Виссарион",
		"Виталий",
		"Владимир",
		"Владислав",
		"Влас,",
		"Всеволод",
		"Всеслав",
		"Вячеслав",
		"Гавриил",
		"Галактион",
		"Гедеон",
		"Гелий",
		"Геннадий",
		"Генрих",
		"Георгий",
		"Герасим",
		"Герман",
		"Гермоген",
		"Геронт",
		"Глеб",
		"Гордей",
		"Григорий",
		"Гурий",
		"Давид",
		"Данила",
		"Дементий",
		"Демид",
		"Демьян",
		"Денис",
		"Див",
		"Дмитрий",
		"Дормидонт",
		"Дорофей",
		"Дружина",
		"Евгений",
		"Евграф",
		"Евдоким",
		"Евлампий",
		"Евсей",
		"Евстафий",
		"Евстигней",
		"Евстрат",
		"Егор,",
		"Елезарий",
		"Елисей",
		"Емельян",
		"Епифан",
		"Еремей",
		"Ермолай",
		"Ерофей",
		"Ефим",
		"Ефрем",
		"Захар",
		"Зиновий",
		"Зосима",
		"Иакинф",
		"Иван",
		"Игнат",
		"Игорь",
		"Иероним",
		"Измаил",
		"Изяслав",
		"Иларий",
		"Илларион",
		"Илья",
		"Иннокентий",
		"Иона",
		"Иосиф",
		"Ипат",
		"Ипполит",
		"Исаак",
		"Сидор",
		"Казимир",
		"Каллистрат",
		"Капитон",
		"Карл",
		"Карп",
		"Касьян",
		"Киприан",
		"Кирилл",
		"Клавдий",
		"Клим",
		"Кондрат",
		"Конон",
		"Константин",
		"Корней",
		"Кузьма",
		"Лавр",
		"Лаврентий",
		"Лазарь",
		"Лев",
		"Леонард",
		"Леонид",
		"Леонтий",
		"Маврикий",
		"Макар",
		"Максим",
		"Максимилиан",
		"Марк",
		"Маркел",
		"Мартин",
		"Матвей",
		"Мелетий",
		"Мелитон",
		"Меркурий",
		"Мефодий",
		"Мирон",
		"Мирослав",
		"Михей",
		"Мстислав",
		"Назар",
		"Нарцисс",
		"Наум",
		"Нестор",
		"Никандр",
		"Никита",
		"Никифор",
		"Никодим",
		"Никон",
		"Нил",
		"Нифонт",
		"Олег",
		"Олимп",
		"Орест",
		"Оскар",
		"Остромир",
		"Павел",
		"Панкрат",
		"Пантелей",
		"Парамон",
		"Парфен",
		"Патрикей",
		"Пахом",
		"Пётр",
		"Пимен",
		"Платон",
		"Поликарп",
		"Помпей",
		"Порфирий",
		"Пров",
		"Прокл",
		"Прокоп",
		"Протас",
		"Прохор",
		"Радим",
		"Радислав",
		"Ратибор",
		"Ратмир",
		"Рафаэль",
		"Рем",
		"Роберт",
		"Родион",
		"Роман",
		"Ростислав",
		"Рубен",
		"Рудольф",
		"Руслан",
		"Рюрик",
		"Савва",
		"Савел,",
		"Самсон",
		"Самуил",
		"Сармат",
		"Святополк",
		"Святослав",
		"Севастьян",
		"Северин",
		"Семен,",
		"Серапион",
		"Серафим",
		"Сергей",
		"Сила",
		"Сильвестр,",
		"Сократ",
		"Соломон",
		"Софрон",
		"Спартак",
		"Спиридон",
		"Станислав",
		"Степан",
		"Тавр",
		"Тарас",
		"Терентий",
		"Тимофей",
		"Тимур",
		"Тит",
		"Тихон",
		"Трифон",
		"Трофим",
		"Фадей",
		"Фалалей",
		"Феодор",
		"Федот",
		"Феликс",
		"Федосей",
		"Феоктист",
		"Феофан,",
		"Феофил",
		"Ферапонт",
		"Филарет",
		"Филимон",
		"Филипп",
		"Фирс",
		"Фома",
		"Фортунат",
		"Фотий",
		"Фрол",
		"Харитон",
		"Христиан",
		"Христофор",
		"Эдуард",
		"Эмиль,",
		"Эразм",
		"Эраст",
		"Эрик",
		"Ювеналий",
		"Юлий",
		"Юрий",
		"Яков,",
		"Ян",
		"Януарий",
		"Ярополк",
		"Ярослав",
		"Ясон",
	};

	#endregion

	public MarkovNameGenerator(IEnumerable<string> sampleNames, int order, int minLength)
	{
		//fix parameter values
		if (order < 1)
		{
			order = 1;
		}
		if (minLength < 1)
		{
			minLength = 1;
		}

		this.order = order;
		this.minLength = minLength;

		//split comma delimited lines
		foreach (string line in sampleNames)
		{
			var tokens = line.Split(',');
			foreach (string word in tokens)
			{
				string upper = word.Trim().ToUpper();
				if (upper.Length < order + 1)
				{
					continue;
				}
				samples.Add(upper);
			}
		}

		//Build chains            
		foreach (string word in samples)
		{
			for (int letter = 0; letter < word.Length - order; letter++)
			{
				string token = word.Substring(letter, order);
				List<char> entry;
				if (chains.ContainsKey(token))
				{
					entry = chains[token];
				}
				else
				{
					entry = new List<char>();
					chains[token] = entry;
				}
				entry.Add(word[letter + order]);
			}
		}
	}

	//Get the next random name
	public string NextName
	{
		get
		{
			//get a random token somewhere in middle of sample word                
			string s;
			do
			{
				int n = rnd.Next(samples.Count);
				int nameLength = samples[n].Length;
				s = samples[n].Substring(rnd.Next(0, samples[n].Length - order), order);
				while (s.Length < nameLength)
				{
					string token = s.Substring(s.Length - order, order);
					char c = GetLetter(token);
					if (c != '?')
					{
						s += GetLetter(token);
					}
					else
					{
						break;
					}
				}

				if (s.Contains(" "))
				{
					var tokens = s.Split(' ');
					s = "";
					for (int t = 0; t < tokens.Length; t++)
					{
						if (tokens[t] == "")
						{
							continue;
						}
						if (tokens[t].Length == 1)
						{
							tokens[t] = tokens[t].ToUpper();
						}
						else
						{
							tokens[t] = tokens[t].Substring(0, 1) + tokens[t].Substring(1).ToLower();
						}
						if (s != "")
						{
							s += " ";
						}
						s += tokens[t];
					}
				}
				else
				{
					s = s.Substring(0, 1) + s.Substring(1).ToLower();
				}
			}
			while (used.Contains(s) || s.Length < minLength);
			used.Add(s);
			return s;
		}
	}

	//Reset the used names
	public void Reset()
	{
		used.Clear();
	}

	//private members
	private readonly Dictionary<string, List<char>> chains = new Dictionary<string, List<char>>();
	private readonly List<string> samples = new List<string>();
	private readonly List<string> used = new List<string>();
	private readonly System.Random rnd = new System.Random();
	private readonly int order;
	private readonly int minLength;

	//Get a random letter from the chain
	private char GetLetter(string token)
	{
		if (!chains.ContainsKey(token))
		{
			return '?';
		}
		var letters = chains[token];
		int n = rnd.Next(letters.Count);
		return letters[n];
	}
}















public class MarkovTest
{
	public static void GenerateName()
	{
		var generator = new MarkovNameGenerator(MarkovNameGenerator.RussianMaleNames, 2, 4);
		Debug.Log(generator.NextName);
		
		/*foreach(string item in generator) {
			string name = generator.NextName;
			Debug.Log(name);
		}*/

		/*while (true)
		{
			for (int i = 0; i < 30; i++)
			{
				string name = generator.NextName;
				if (name.EndsWith("и") || name.EndsWith("е"))
				{
					name += "й";
				}
				else if (name.EndsWith("о"))
				{
					name += "н";
				}

				Console.WriteLine(name);
			}
			Console.WriteLine();
			Console.ReadLine();
		}*/
	}
}