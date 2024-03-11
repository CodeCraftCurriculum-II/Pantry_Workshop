using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using PayLoad = System.Collections.Generic.Dictionary<string, string>;

const string CONFIG_FILE = ".config";
const string DEFAULT_BASKET = "save";
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
        Console.WriteLine($"Pantry: {pantry.name}");
        BasketListing? saveBasket = null;
        foreach (BasketListing basket in pantry.baskets)
        {
            Console.WriteLine($"- {basket.name}");
            if (basket.name == DEFAULT_BASKET)
            {
                saveBasket = basket;
            }
        }

        if (saveBasket == null)
        {
            respons = await httpClient.PostAsJsonAsync(basketURL, new PayLoad() { });
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


