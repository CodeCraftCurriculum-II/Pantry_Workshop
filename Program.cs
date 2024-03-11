using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Content = System.Collections.Generic.Dictionary<string, string>;

const string CONFIG_FILE = ".config";
const string DEFAULT_BASKET = "save";
const string DEFAULT_KEY = "note_";
const string KEY_PARAMETER = "-n";
string pantry_id = "";

if (File.Exists(CONFIG_FILE))
{
    string[] configDetails = File.ReadAllLines(CONFIG_FILE);
    pantry_id = configDetails[0];
}
else
{
    Console.Write("Input Pantry Id : ");
    pantry_id = Console.ReadLine() ?? "";
    if (pantry_id.Length > 0)
    {
        File.WriteAllText(CONFIG_FILE, pantry_id);
    }
}

HttpClient httpClient = InitializeHttpClient();
string baseURL = $"https://getpantry.cloud/apiv1/pantry/{pantry_id}/";
string basketURL = $"{baseURL}basket/save";

HttpResponseMessage respons = await httpClient.GetAsync(baseURL);

if (respons.StatusCode == System.Net.HttpStatusCode.OK)
{
    string pantryRawContent = (await respons.Content.ReadAsStringAsync()) ?? "";
    Pantry? pantry = JsonSerializer.Deserialize<Pantry>(pantryRawContent);

    if (pantry != null)
    {
        BasketListing? saveBasket = null;
        if(pantry?.baskets != null){
        foreach (BasketListing basket in pantry.baskets)
        {
            if (basket.name == DEFAULT_BASKET)
            {
                saveBasket = basket;
            }
        }
        }

        if (saveBasket == null)
        {
            respons = await httpClient.PostAsJsonAsync(basketURL, new Content() { });
            Console.WriteLine(await respons.Content.ReadAsStringAsync());
        }
    }
    else
    {
        Console.WriteLine("Error: Could not get Pantry");
    }
}
else
{
    Console.WriteLine(respons.StatusCode);
    Console.WriteLine(await respons.Content.ReadAsStringAsync());
}


respons = await httpClient.GetAsync(basketURL);
Content? allNotes = JsonSerializer.Deserialize<Content>(await respons.Content.ReadAsStringAsync());

if (args.Length > 0)
{
    string key = "";
    int startIndex = 0;

    for (int i = 0; i < args.Length; i++)
    {
        if (args[i] == KEY_PARAMETER)
        {
            key = args[i + 1];
            startIndex = i + 2;
            break;
        }
    }

    string content = String.Join(" ", args[startIndex..args.Length]);

    if (key == "")
    {
        int index = 0;
        do
        {
            key = $"{DEFAULT_KEY}{index}";
            index++;
        } while (allNotes != null && allNotes.ContainsKey(key));
    }


    respons = await httpClient.PutAsJsonAsync(basketURL, new Content() { { key, content } });
    if (respons.StatusCode == System.Net.HttpStatusCode.OK)
    {
        Console.WriteLine("Saved");
    }
    else
    {
        Console.WriteLine(await respons.Content.ReadAsStringAsync());
    }

} else {

    if(allNotes != null){
        foreach (var pair in allNotes)
        {
            Console.WriteLine(pair.Key);
            Console.WriteLine(pair.Value);
            Console.WriteLine("");
        }
    }

}



HttpClient InitializeHttpClient()
{
    HttpClient httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return httpClient;
}

class Pantry
{
    public string? name { get; set; }
    public string? description { get; set; }
    public string[]? errors { get; set; }
    public bool? notifications { get; set; }
    public double? percentFull { get; set; }
    public BasketListing[]? baskets { get; set; }
}

class BasketListing
{
    public string? name { get; set; }
    public int? ttl { get; set; }
}


