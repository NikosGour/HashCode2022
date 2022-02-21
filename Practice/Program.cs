using System.Diagnostics;
using System.Text;

namespace Practice;

static class Files
{
    public const string A = "a_an_example.in.txt";
    public const string B = "b_basic.in.txt";
    public const string C = "c_coarse.in.txt";
    public const string D = "d_difficult.in.txt";
    public const string E = "e_elaborate.in.txt";
}

public static class Program
{
    static readonly string ASSEMBLY_PATH    = AppDomain.CurrentDomain.BaseDirectory;
    static readonly string PROJECT_DIR_PATH = ASSEMBLY_PATH[..ASSEMBLY_PATH.IndexOf("\\bin", StringComparison.Ordinal)];
    static readonly string INPUTS_DIR_PATH  = Path.Combine(PROJECT_DIR_PATH, "inputs");

    public static void Main()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        Directory.SetCurrentDirectory(INPUTS_DIR_PATH);
        const string FILE = Files.D;

        Customer?[]             customers;
        Dictionary<string, int> liked_ingredients    = new();
        Dictionary<string, int> disliked_ingredients = new();

        #region parsing

        using (var file = new StreamReader(FILE))
        {
            string line = file.ReadLine()!;

            var n = int.Parse(line);

            customers = new Customer[n];

            for (int i = 0; i < n; i++)
            {
                line = file.ReadLine()!;
                string[] words = line.Split(' ');

                string[] customer_l = new string[int.Parse(words[0])];

                if (int.Parse(words[0]) != 0)
                {
                    for (int j = 1; j < words.Length; j++)
                    {
                        customer_l[j - 1] = words[j];
                    }

                    liked_ingredients.form(words);
                }

                line  = file.ReadLine()!;
                words = line.Split(' ');

                string[] customer_d = new string[int.Parse(words[0])];

                if (int.Parse(words[0]) != 0)
                {
                    for (int j = 1; j < words.Length; j++)
                    {
                        customer_d[j - 1] = words[j];
                    }

                    disliked_ingredients.form(words);
                }

                customers[i] = new Customer(customer_l, customer_d);
            }
        }

        #endregion

        stopwatch.Stop();
        Console.WriteLine($"Parsing time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine("Finished parsing");


        var ingredients = new List<string>();


        //Sorting
        liked_ingredients =
            (from dict in liked_ingredients orderby dict.Value descending select dict).ToDictionary(
                dict => dict.Key,
                dict => dict.Value);


        List<Customer> customers_to_remove = new();

        // TODO : CUSTOMER CLASS REMEMBER TO REMOVE DISSATISFIED CUSTOMERS' INGREDIENTS

        foreach (var ingredient in liked_ingredients)
        {
            if (disliked_ingredients.ContainsKey(ingredient.Key))
            {
                if (ingredient.Value > disliked_ingredients[ingredient.Key])
                {
                    ingredients.Add(ingredient.Key);

                    foreach (var customer in customers)
                    {
                        if (customer is null)
                        {
                            continue;
                        }

                        if (customer.disliked_ingredients.Contains(ingredient.Key))
                        {
                            foreach (var customer_ingredient in customer.liked_ingredients)
                            {
                                liked_ingredients[customer_ingredient]--;
                            }

                            foreach (var customer_ingredient in customer.disliked_ingredients)
                            {
                                disliked_ingredients[customer_ingredient]--;
                            }

                            customers_to_remove.Add(customer);
                        }
                    }

                    foreach (var customer in customers_to_remove)
                    {
                        customers[customers_to_remove.IndexOf(customer)] = null;
                    }

                    customers_to_remove.Clear();
                }
            }
            else
            {
                ingredients.Add(ingredient.Key);
            }
        }

        string output = $"{ingredients.Count}";
        foreach (var ingredient in ingredients)
        {
            output += $" {ingredient}";
        }

        using var file_out = new StreamWriter($"../outputs/{FILE}.out.txt");
        file_out.Write(output);

        int sum = 0;

        foreach (var customer in customers)
        {
            if (customer is not null)
            {
                sum++;
            }
        }

        file_out.Write($"\n{sum}");
        stopwatch.Stop();
        Console.WriteLine($"Single Ingredient time: {stopwatch.ElapsedMilliseconds}");
    }

    private static void form(this Dictionary<string, int> ingredients, string[] words)
    {
        for (int i = 1; i < words.Length; i++)
        {
            if (ingredients.ContainsKey(words[i]))
            {
                ingredients[words[i]]++;
            }
            else
            {
                ingredients.Add(words[i], 1);
            }
        }
    }

    record Customer(string[] liked_ingredients, string[] disliked_ingredients);
}