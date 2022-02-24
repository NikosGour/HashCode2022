namespace Qualification;



internal static class Files
{
    public const string A = "a_an_example.in.txt";
    public const string B = "b_better_start_small.in.txt";
    public const string C = "c_collaboration.in.txt";
    public const string D = "d_dense_schedule.in.txt";
    public const string E = "e_exceptional_skills.in.txt";
    public const string F = "f_find_great_mentors.in.txt";
}
internal static class Program
{

    static readonly string ASSEMBLY_PATH    = AppDomain.CurrentDomain.BaseDirectory;
    static readonly string PROJECT_DIR_PATH = ASSEMBLY_PATH.Substring(0, ASSEMBLY_PATH.IndexOf("\\bin"));
    static readonly string INPUTS_DIR_PATH  = Path.Combine(PROJECT_DIR_PATH, "inputs");

    public static void Main()
    {
        Directory.SetCurrentDirectory(INPUTS_DIR_PATH);

        const string FILE = Files.A;

        using (var file = new StreamReader(FILE))
        {
            Console.WriteLine();
        }

    }
}