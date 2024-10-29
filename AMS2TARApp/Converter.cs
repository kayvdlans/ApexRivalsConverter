using System.IO;
using System.Text.Json;
using System.Xml.Linq;

namespace AMS2ToApexRivals;

public class Converter(string inputPath, string outputDir, string defaultValue = "Random")
{
	public void Convert() 
	{
		var options = new JsonSerializerOptions { WriteIndented = true };

		Directory.CreateDirectory(outputDir);

		var root = XElement.Load(inputPath);
		foreach (var driver in root.Elements("driver")) 
		{ 
			var name = driver.Element("name")?.Value ?? "Error";
            var country = driver.Element("country")?.Value ?? "Random";

			var ams2 = new 
			{
				Race_Skill = GetValueOrDefault(driver, "race_skill"),
				Qualifying_Skill = GetValueOrDefault(driver, "qualifying_skill"),
				Aggression = GetValueOrDefault(driver, "aggression"),
				Defending = GetValueOrDefault(driver, "defending"),
				Stamina = GetValueOrDefault(driver, "stamina"),
				Consistency = GetValueOrDefault(driver, "consistency"),
				Tyre_Change = GetValueOrDefault(driver, "weather_tyre_changes"),
				Start_Reaction = GetValueOrDefault(driver, "start_reactions"),
				Wet_Skill = GetValueOrDefault(driver, "wet_skill"),
				Tyre_Management = GetValueOrDefault(driver, "tyre_management"),
				Fuel_Management = GetValueOrDefault(driver, "fuel_management"),
				Blue_Flag = GetValueOrDefault(driver, "blue_flag_conceding"),
				Mistake_Avoidance = GetValueOrDefault(driver, "avoidance_of_mistakes"),
				Forced_Avoidance = GetValueOrDefault(driver, "avoidance_of_forced_mistakes")
			};

			var json = new 
			{
				Name = name,
				Country = country,
				AMS2 = ams2,
				AC = new { Strength = "Random", Aggressivity = "Random" }
			};

			var jsonOutput = JsonSerializer.Serialize(json, options).Replace('_', ' ');
			var outputPath = Path.Combine(outputDir, name);
			Directory.CreateDirectory(outputPath);
			outputPath = Path.Combine(outputPath, $"{name}.json");
			File.WriteAllText(outputPath, jsonOutput);
			Console.WriteLine($"Saved JSON for {name} at {outputPath}");
		}

	}

	private string GetValueOrDefault(XElement root, string element) 
	{
		return root.Element(element)?.Value ?? defaultValue;
	}
}