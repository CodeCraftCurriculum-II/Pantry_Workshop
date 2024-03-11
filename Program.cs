const string CONFIG_FILE = ".config";

string pantry_id = ExtractPantryID(ReadFile(CONFIG_FILE)) ?? SaveContentToFile(CONFIG_FILE, QueryForPantryID());


string? QueryForPantryID(){

    Console.Write("Pantry ID: ");
    return Console.ReadLine();

}

string? ExtractPantryID(string[]? configSettings){
    if(configSettings?.Length > 0){
        return configSettings[0];
    }
    return null;
}

string[]? ReadFile(string filePath)
{
    string[]? output = null;

    try
    {
        if (File.Exists(filePath))
        {
            output = File.ReadAllLines(filePath);
        } else{
            throw new FileNotFoundException();
        }
    }
    catch (System.Exception e)
    {
        Console.WriteLine(e.Message);
    }

    return output;
}

string SaveContentToFile(string filePath, string? text){

    try
    {
        if(text != null){
            File.WriteAllText(filePath,text);
        }
    }
    catch (System.Exception e)
    {
        Console.WriteLine(e.Message);    
    }

    return text;

}