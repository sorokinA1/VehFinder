// var vehFinderApp = new VehFinderApp(new VehiclesRepository(new StringsTextualRepository()));
// vehFinderApp.Init();




void ClickQuickSearchSimulator()
{
    Console.WriteLine("Click");
}


public class VehFinderApp
{
    private readonly IVehRepository _vehRepository;

    public VehFinderApp(IVehRepository vehRepository)
    {
        _vehRepository = vehRepository;
    }

    public void Init()
    {
       var x = _vehRepository.Read("vehicles.txt");

       foreach (var s in x)
       {
           Console.WriteLine(s);
       }
    }
}

public interface IVehRepository
{
    List<string> Read(string filePath);
}

public class VehiclesRepository : IVehRepository
{
    
    private readonly IStringsRepository _stringsRepository;

    
    public VehiclesRepository(IStringsRepository stringsRepository)
    {
        _stringsRepository = stringsRepository;
    }
    
    
    public List<string> Read(string filePath)
    {
        List<string> VehiclesFromFile = _stringsRepository.Read(filePath);
        var vehicles = new List<string>();

        foreach (var vehicle in VehiclesFromFile)
        {
            vehicles.Add(vehicle);
        }

        return vehicles;
    }
}

public interface IStringsRepository
{
    List<string> Read(string filePath);
}

public abstract class StringsRepository : IStringsRepository
{
    public List<string> Read(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("File vehicles.txt is not found");
        var fileContents = File.ReadAllText(filePath);
        return TextToStrings(fileContents);
    }

    protected abstract List<string> TextToStrings(string fileContents);
}

public class StringsTextualRepository : StringsRepository
{
    private static readonly string Separator = Environment.NewLine;

    protected override List<string> TextToStrings(string fileContents)
    {
        return fileContents.Split(Separator).ToList();
    }
}

