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
        Directory.SetCurrentDirectory(INPUTS_DIR_PATH);
        const string FILE = Files.E;

        Dictionary<string, int> liked_ingredients    = new();
        Dictionary<string, int> disliked_ingredients = new();
        var                     liked_mode           = true;

        #region parsing

        using (var file = new StreamReader(FILE))
        {
            var line = file.ReadLine();

            while ((line = file.ReadLine()) != null)
            {
                var words = line.Split(' ');
                if (words.Length == 1)
                {
                    liked_mode = !liked_mode;
                    continue;
                }


                var dict = liked_mode ? liked_ingredients : disliked_ingredients;
                form_dictionary(dict, words);

                liked_mode = !liked_mode;
            }
        }

        #endregion

        var ingredients = new List<string>();

        //Sorting
        liked_ingredients =
            (from dict in liked_ingredients orderby dict.Value descending select dict).ToDictionary(
                dict => dict.Key,
                dict => dict.Value);


        // TODO : CUSTOMER CLASS REMEMBER TO REMOVE DISSATISFIED CUSTOMERS' INGREDIENTS

        foreach (var ingredient in liked_ingredients)
        {
            if (disliked_ingredients.ContainsKey(ingredient.Key))
            {
                if (ingredient.Value > disliked_ingredients[ingredient.Key])
                {
                    ingredients.Add(ingredient.Key);
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
    }

    private static void form_dictionary(Dictionary<string, int> ingredients, string[] words)
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
}