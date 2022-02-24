using System.Diagnostics;

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
        Main2();
        Console.ReadLine();
    }

    public static void Main2()
    {
        Directory.SetCurrentDirectory(INPUTS_DIR_PATH);

        const string FILE = Files.B;

        int           num_of_contributors, num_of_projects;
        Contributor[] contributors;
        Project[]     projects;

        Stopwatch sw = Stopwatch.StartNew();

        #region parsing

        using (var file = new StreamReader(FILE))
        {
            var first_line = file.ReadLine()!.Split(' ');

            num_of_contributors = int.Parse(first_line[0]);
            num_of_projects     = int.Parse(first_line[1]);


            contributors = new Contributor[num_of_contributors];
            projects     = new Project[num_of_projects];

            for (int i = 0; i < num_of_contributors; i++)
            {
                var name_and_skills_s = file.ReadLine()!.Split(' ');

                var name          = name_and_skills_s[0];
                var num_of_skills = int.Parse(name_and_skills_s[1]);

                Contributor contributor = new(name);

                for (int j = 0; j < num_of_skills; j++)
                {
                    var skill_s = file.ReadLine()!.Split(' ');
                    contributor.skills.Add(new Skill(skill_s[0], int.Parse(skill_s[1])));
                }

                contributors[i] = contributor;
            }

            for (int i = 0; i < num_of_projects; i++)
            {
                var first_line_s = file.ReadLine()!.Split(' ');

                var name                       = first_line_s[0];
                var number_of_days_to_complete = int.Parse(first_line_s[1]);
                var score                      = int.Parse(first_line_s[2]);
                var best_before                = int.Parse(first_line_s[3]);
                var num_of_skills              = int.Parse(first_line_s[4]);

                Project project = new(name, number_of_days_to_complete, score, best_before);

                for (int j = 0; j < num_of_skills; j++)
                {
                    var skill_s = file.ReadLine()!.Split(' ');
                    project.skills.Add(new Skill(skill_s[0], int.Parse(skill_s[1])));
                }

                project.contributors = new Contributor[project.skills.Count];
                projects[i]          = project;
            }
        }

        #endregion

        Console.WriteLine($"{sw.ElapsedMilliseconds} ms");


        projects = (from project in projects
                    orderby project.days_to_complete
                    select project).ToArray();
        int completed_projects = 0;
        int days               = 0;


        string output = "";
        do
        {
            for (int i = 0; i < num_of_projects; i++)
            {
                Project project = projects[i];

                if (project is null)
                {
                    continue;
                }

                for (int j = 0; j < project.skills.Count; j++)
                {
                    Skill skill = project.skills[j];
                    for (int k = 0; k < num_of_contributors; k++)
                    {
                        Contributor contributor = contributors[k];

                        if (contributor.skills.Contains(skill))
                        {
                            if (contributor.skills.Find(x => x.name == skill.name)!.level >= skill.level)
                            {
                                if (!contributor.is_occupied)
                                {
                                    project.contributors[j] = contributor;
                                    contributor.is_occupied = true;
                                    break;
                                }
                            }
                        }
                    }
                }


                if (project.contributors.All(x => x is not null))
                {
                    project.running = true;
                }

                // Means we havent filled the roles
                else
                {
                    for (int j = 0; j < project.contributors.Length; j++)
                    {
                        Contributor contributor = project.contributors[j];

                        if (contributor is not null)
                        {
                            continue;
                        }

                        Skill skill = project.skills[j];

                        var mentors = (from c in project.contributors
                                       where c is not null
                                          && c.skills.Contains(skill)
                                          && c.skills.Find(x => x.name == skill.name)!.level >= skill.level
                                       select c).ToArray();

                        if (mentors.Length > 0)
                        {
                            Contributor mentor = mentors[0];

                            foreach (var potential_contributor in contributors)
                            {
                                if (skill.level == 1)
                                {
                                    Contributor less_capable_contr = contributors.ToList()
                                        .Find(y => y.skills.Count
                                                == contributors.ToList()
                                                               .Min(x => x.skills.Count))!;

                                    project.contributors[j]        = less_capable_contr;
                                    less_capable_contr.is_occupied = true;
                                    break;
                                }

                                if (potential_contributor.skills.Contains(skill))
                                {
                                    if (potential_contributor.skills.Find(x => x.name == skill.name)!.level
                                     <= skill.level - 1)
                                    {
                                        if (!potential_contributor.is_occupied)
                                        {
                                            project.contributors[j]           = potential_contributor;
                                            potential_contributor.is_occupied = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (project.contributors.All(x => x is not null))
                    {
                        project.running = true;
                    }
                    else
                    {
                        project.running = false;
                        for (var j = 0; j < project.contributors.Length; j++)
                        {
                            var contributor = project.contributors[j];
                            if (contributor is not null)
                            {
                                contributor.is_occupied = false;
                                project.contributors[j] = null;
                            }
                        }
                    }
                }


                if (project.running)
                {
                    project.days_to_complete--;
                    if (project.days_to_complete == 0)
                    {
                        output += project.name + "\n";
                        for (var j = 0; j < project.contributors.Length; j++)
                        {
                            var contributor = project.contributors[j];
                            output += $"{contributor.name} ";


                            Skill project_skill = project.skills[j];
                            if (contributor.skills.Contains(project_skill))
                            {
                                if (project_skill.level
                                 >= contributor.skills.Find(x => x.name == project_skill.name)!.level)
                                {
                                    contributor.skills.Find(x => x.name == project_skill.name)!.level++;
                                }
                            }
                            else
                            {
                                contributor.skills.Add(new Skill(project_skill.name, 1));
                            }


                            contributor.is_occupied = false;
                            project.contributors[j] = null;
                        }

                        output += "\n";


                        completed_projects++;
                        projects[i] = null;
                    }
                }
            }

            Console.WriteLine(days);
            days++;
        } while (days < 5000); //while (completed_projects < num_of_projects);

        Console.WriteLine("Finished\n~~~~~~~~~~~~");

        sw.Stop();
        Console.WriteLine($"{sw.ElapsedMilliseconds} ms");

        using (var file = new StreamWriter($"../outputs/{FILE}.out")) file.Write($"{completed_projects}\n{output}");
    }
}

internal class Contributor
{
    public string      name        { get; set; }
    public bool        is_occupied { get; set; } = false;
    public List<Skill> skills      { get; set; } = new();

    public Contributor(string name)
    {
        this.name = name;
    }
}

internal class Project
{
    public string name             { get; set; }
    public int    days_to_complete { get; set; }
    public int    score            { get; set; }
    public int    best_before      { get; set; }

    public bool running { get; set; } = false;

    public Contributor[] contributors { get; set; }
    public List<Skill>   skills       { get; set; } = new();

    public Project(string name, int daysToComplete, int score, int bestBefore)
    {
        this.name        = name;
        days_to_complete = daysToComplete;
        this.score       = score;
        best_before      = bestBefore;
    }
}

record Skill()
{
    public string name  { get; set; }
    public int    level { get; set; }

    public Skill(string name, int level) : this()
    {
        this.name  = name;
        this.level = level;
    }

    public virtual bool Equals(Skill? other)
    {
        return this.name == other!.name;
    }
}