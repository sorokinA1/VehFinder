var vehFinderApp = new VehFinderApp(
    new DataManipulator(new VehiclesRepository(
        new StringsTextualRepository())));


var filterSettings = new List<string> { "usa", "LT", "SPG" };

var selectedVehicle = "";


var countries = vehFinderApp.GetComboBoxOptions(DataField.VehCountryField);

foreach (var country in countries)
{
    Console.WriteLine(country);
}
/*
// var placeToSearch = DataField.VehNameField;
// var searchParams = new List<string> { "usa", "LT", "10" };
// Ho_Ri_3, japan, AT-SPG, 10, 24096

var controller = new DataManipulator(new VehiclesRepository(new StringsTextualRepository()));
// var x = controller.FindItem("Ho", DataField.VehNameField);

// var x = controller.FindUniqueItems(DataField.VehCountryField);
// var x = controller.FindItem(new List<string> { "usa", "SPG", "10" });
var x = controller.FindItem("94281", DataField.VehIdField);
var selectedVehicle = x[0].Length;
Console.WriteLine(selectedVehicle);
var selected = x[0];

foreach (var s in selected)
{
    Console.Write(s + " ");
}

Console.WriteLine("");
controller.UpdateLabel(selected, "Autoloader");

foreach (var s in selected)
{
    Console.Write(s + " ");
}

// foreach (var s in selectedVehicle)
// {
//     Console.Write(s + " | ");
// }

// foreach (var s in x)
// {
//     foreach (var f in s)
//     {
//         Console.Write(f + ", ");
//     }
//
//     Console.WriteLine("");
// }

// var test = new VehiclesRepository(new StringsTextualRepository());
// var xa = test.CreateVehicleCollection(test.Read("vehicles.txt"));
// test.Write("tanks.txt", xa);
// test.Write("tanks.txt", );


// var country = vehFinderApp.GetComboBoxOptions(DataField.VehCountryField);
// var level = vehFinderApp.GetComboBoxOptions(DataField.VehLevelField);
// var type = vehFinderApp.GetComboBoxOptions(DataField.VehTypeField);


// test
// var repo = new DataManipulator(new VehiclesRepository
//     (new StringsTextualRepository()), new IteratorHelper());
// var y = repo.FindItem(new List<string> { "japan", "", "10" });
// foreach (var x in y)
// {
//     Console.WriteLine(x);
// }
// var x = repo.FindItem("Lat", isSearchById);
// var y = repo.FindItem(DataField.VehLevelField);
// foreach (var s in y)
// {
//     Console.WriteLine(s);
// }
*/

void ClickQuickSearchSimulator()
{
}

// void ClickQuickChangeFirstSearchParam()
// {
//     searchParams[0] = "UDES";
// }


public class VehFinderApp
{
    private readonly IDataManipulator _dataManipulator;

    public VehFinderApp(IDataManipulator dataManipulator)
    {
        _dataManipulator = dataManipulator;
    }

    public List<string> GetComboBoxOptions(DataField dataField)
    {
        return _dataManipulator.FindUniqueItems(dataField);
    }
}

public enum SearchParams
{
    NoParams = 0b0000,
    Country = 0b0001,
    Type = 0b0010,
    Level = 0b0100,
    CountryAndType = 0b0011,
    CountryAndLevel = 0b0101,
    TypeAndLevel = 0b0110,
    CountryAndLevelAndType = 0b0111
}

public enum DataField
{
    VehNameField,
    VehCountryField,
    VehTypeField,
    VehLevelField,
    VehIdField,
    VehLabelField
}

public interface IDataManipulator
{
    List<string[]> FindItem(string vehicleName, DataField searchField);
    List<string> FindUniqueItems(DataField dataField);
    List<string[]> FindItem(List<string> searchParams);
}

public class DataManipulator : IDataManipulator
{
    private readonly List<string[]> _vehicleCollection;
    private readonly IVehRepository _vehRepo;

    public DataManipulator(IVehRepository vehRepo)
    {
        _vehRepo = vehRepo;
        _vehicleCollection = _vehRepo.CreateVehicleCollection(_vehRepo.Read("vehicles.txt"));
    }

    public void UpdateLabel(string[] selectedVehicle, string updatedLabel)
    {
        var vehArrayIndex = _vehicleCollection.IndexOf(selectedVehicle);
        _vehicleCollection[vehArrayIndex][^1] = updatedLabel;
        _vehRepo.Write("vehicles.txt", _vehicleCollection);
    }

    public List<string> FindUniqueItems(DataField dataField)
    {
        var allUniqueValues = new HashSet<string>();

        foreach (var value in _vehicleCollection)
        {
            allUniqueValues.Add(value[(int)dataField]);
        }

        return allUniqueValues.ToList();
    }

    public List<string[]> FindItem(string vehicleName, DataField searchField)
    {
        return IteratorHelper.Iterate(
            _vehicleCollection, new List<string> { vehicleName }, searchField);
    }

    public List<string[]> FindItem(List<string> searchParams)
    {
        var searchNamesList = new List<string>();

        var searchParameter = (int)SearchParams.NoParams;

        for (int i = 0; i < searchParams.Count; i++)
        {
            // 1.country, 2.type, 3.level
            if (searchParams[i] == "") continue;
            searchParameter |= 1 << i;
            searchNamesList.Add(searchParams[i]);
        }

        return searchParameter switch
        {
            // Console.WriteLine("Show VehCountry and VehType");
            (int)SearchParams.NoParams => IteratorHelper.Iterate(_vehicleCollection),
            (int)SearchParams.Country => IteratorHelper.Iterate(_vehicleCollection, searchNamesList,
                DataField.VehCountryField),
            (int)SearchParams.Type => IteratorHelper.Iterate(_vehicleCollection, searchNamesList,
                DataField.VehTypeField),
            (int)SearchParams.Level => IteratorHelper.Iterate(_vehicleCollection, searchNamesList,
                DataField.VehLevelField),
            (int)SearchParams.CountryAndType => IteratorHelper.Iterate(_vehicleCollection, searchNamesList,
                DataField.VehCountryField, DataField.VehTypeField),
            (int)SearchParams.CountryAndLevel => IteratorHelper.Iterate(_vehicleCollection, searchNamesList,
                DataField.VehCountryField, DataField.VehLevelField),
            (int)SearchParams.TypeAndLevel => IteratorHelper.Iterate(_vehicleCollection, searchNamesList,
                DataField.VehTypeField, DataField.VehLevelField),
            (int)SearchParams.CountryAndLevelAndType => IteratorHelper.Iterate(_vehicleCollection, searchNamesList,
                DataField.VehCountryField, DataField.VehTypeField, DataField.VehLevelField),
            _ => new List<string[]>(),
        };
    }
}

public interface IVehRepository
{
    List<string> Read(string filePath);
    void Write(string filePath, List<string[]> allVehicles);
    List<string[]> CreateVehicleCollection(List<string> vehicleList);
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
        List<string> vehiclesFromFile = _stringsRepository.Read(filePath);
        var vehicles = new List<string>();

        foreach (var vehicle in vehiclesFromFile)
        {
            vehicles.Add(vehicle);
        }

        return vehicles;
    }

    public void Write(string filePath, List<string[]> allVehicles)
    {
        var vehiclesAsStrings = new List<string>();

        foreach (var vehicle in allVehicles)
        {
            vehiclesAsStrings.Add(string.Join("|", vehicle));
        }

        _stringsRepository.Write(filePath, vehiclesAsStrings);
    }

    public List<string[]> CreateVehicleCollection(List<string> vehicleList)
    {
        var vehicleCollection = new List<string[]>();

        foreach (var vehicle in vehicleList)
        {
            var extractedInfo = vehicle.Split("|");
            vehicleCollection.Add(extractedInfo);
        }

        return vehicleCollection;
    }
}

public interface IStringsRepository
{
    List<string> Read(string filePath);
    void Write(string filePath, List<string> strings);
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

    public void Write(string filePath, List<string> strings)
    {
        File.WriteAllText(filePath, StringsToText(strings));
    }

    protected abstract string StringsToText(List<string> strings);
}

public class StringsTextualRepository : StringsRepository
{
    private static readonly string Separator = Environment.NewLine;

    protected override List<string> TextToStrings(string fileContents)
    {
        return fileContents.Split(Separator).ToList();
    }

    protected override string StringsToText(List<string> strings)
    {
        return string.Join(Separator, strings);
    }
}

public static class IteratorHelper
{
    public static List<string[]> Iterate(List<string[]> collection, List<string> itemName, params DataField[] dataField)
    {
        var itemsFound = new List<string[]>();
        var paramsCount = itemName.Count;

        foreach (var item in collection)
        {
            switch (paramsCount)
            {
                case 1:
                    if (item[(int)dataField[0]].Contains(itemName[0]))
                    {
                        itemsFound.Add(item);
                    }

                    break;

                case 2:
                    if (item[(int)dataField[0]].Contains(itemName[0]) &&
                        item[(int)dataField[1]].Contains(itemName[1]))
                    {
                        itemsFound.Add(item);
                    }

                    break;

                case 3:
                    if (item[(int)dataField[0]].Contains(itemName[0]) &&
                        item[(int)dataField[1]].Contains(itemName[1]) &&
                        item[(int)dataField[2]].Contains(itemName[2]))
                    {
                        itemsFound.Add(item);
                    }

                    break;

                default:
                    itemsFound.Add(item);
                    break;
            }
        }

        return itemsFound;
    }

    public static List<string[]> Iterate(List<string[]> collection)
    {
        var itemsFound = new List<string[]>();

        foreach (var item in collection)
        {
            itemsFound.Add(item);
        }

        return itemsFound;
        // return collection.Select(item => string.Join(", ", item)).ToList();
    }
}