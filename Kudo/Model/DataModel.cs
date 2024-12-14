namespace Kudo.Model;

public class DataModel
{
    public string Name { get; set; }
    public int ID { get; set; }

    public override string ToString()
    {
        return Name;
    }
}