using CommandLine;


namespace WebClient;


public class Options
{
    [Option('g', "get",
        HelpText = "get customer with specified id")]
    public long? Id { get; set; }

    [Option('p', "post",
        HelpText = "post random generated customer")]
    public bool PostRequired { get; set; }

}