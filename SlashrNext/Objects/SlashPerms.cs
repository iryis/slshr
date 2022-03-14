using DSharpPlus.SlashCommands;

namespace SlashrNext.Objects;

public class SlashRequireMod : SlashCheckBaseAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        var check = await Task.Run(() =>
            ctx.Member.Roles.Contains(ctx.Guild.GetRole(733481223992639650)) ||
            ctx.Member.Roles.Contains(ctx.Guild.GetRole(803735064265490452)) ||
            ctx.Member.Roles.Contains(ctx.Guild.GetRole(745458624972980234)) ||
            ctx.Member.Roles.Contains(ctx.Guild.GetRole(766287875653894185)) || // remove this after testing
            ctx.Member.Roles.Contains(ctx.Guild.GetRole(812555422263148584)));
        return check;
    }
}