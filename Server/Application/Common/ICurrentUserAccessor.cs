namespace MyApp.Server.Application.Common;

public interface ICurrentUserAccessor
{
    CurrentUserInfo GetRequiredCurrentUser();
}
