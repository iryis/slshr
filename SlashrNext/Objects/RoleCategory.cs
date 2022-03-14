using DSharpPlus.Entities;
using SlashrNext.Utils;

namespace SlashrNext.Objects;

public class RoleCategory
{
    public string name;
    
    public string description;
    
    // role, custom role class with emoji and description
    public List<KeyValuePair<DiscordRole, CustomRole>> roles = new();
    

    public RoleCategory(string name, string desc)
    {
        this.name = name;
        description = desc;
    }

    public bool AddRole(DiscordRole role, string desc, DiscordComponentEmoji emoji)
    {
        if (Exists(role)) return false;
        var customRole = new CustomRole(role, desc, emoji);
        roles.Add(new KeyValuePair<DiscordRole, CustomRole>(role, customRole));
        return true;
    }

    public void RemoveRole(DiscordRole role)
    {
        try
        {
            roles.Remove(roles.FirstOrDefault(r => r.Key.Id == role.Id));
        }
        catch (Exception ec)
        {
            Logger.Error($"Could not remove role {role.Name} from category {name}: {ec}");
        }
    }

    public CustomRole GetRole(string rn)
    {
        return roles.FirstOrDefault(r =>
            string.Equals(r.Key.Name, rn, StringComparison.CurrentCultureIgnoreCase)).Value;
    }

    private bool Exists(SnowflakeObject role)
    {
        return roles.Any(r => r.Key.Id == role.Id);
    }
}