namespace MvcLaptop.Utils.Constants;

public static class IconMapping
{
    public static readonly Dictionary<string, string> ActionIcons = new Dictionary<string, string>
    {
        { Constants.CommandCode.CREATE.ToString(), "bi bi-plus-circle" },
        { Constants.CommandCode.VIEW.ToString(), "bi bi-eye" },
        { Constants.CommandCode.UPDATE.ToString(), "bi bi-pencil" },
        { Constants.CommandCode.DELETE.ToString(), "bi bi-trash" },
        { Constants.CommandCode.APPROVE.ToString(), "bi bi-check-circle" },
        { Constants.CommandCode.REJECT.ToString(), "bi bi-x-circle" },
        // Add more actions and their corresponding icons here
    };
}