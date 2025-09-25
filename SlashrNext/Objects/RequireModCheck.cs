using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace SlashrNext.Objects;

public sealed class RequireModCheck : IContextCheck<RequireModAttribute>
{
    public async ValueTask<string?> ExecuteCheckAsync(RequireModAttribute attribute, CommandContext ctx)
    {
        if (ctx.Member is null || ctx.Guild is null)
        {
            return await ValueTask.FromResult<string?>("This command can only be used in a server.");
        }

        var check = await Task.Run(async () =>
            ctx.Member.Roles.Contains(await ctx.Guild.GetRoleAsync(733481223992639650)) ||
            ctx.Member.Roles.Contains(await ctx.Guild.GetRoleAsync(803735064265490452)) ||
            ctx.Member.Roles.Contains(await ctx.Guild.GetRoleAsync(745458624972980234)) ||
            ctx.Member.Roles.Contains(await ctx.Guild.GetRoleAsync(812555422263148584)));
        return await (check
            ? ValueTask.FromResult<string?>(null)
            : ValueTask.FromResult<string?>("This command is only available to server staff."));
    }
}