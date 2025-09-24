using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Common;

public static class VisibilityHelper
{
    public static bool ShouldShowAdminMenu(Associado? user)
    {
        return user != null && user.IdPerfil > 2;
    }

    public static bool ShouldShowEvaluationActions(Associado? user)
    {
        return user != null && user.IdPerfil > 1;
    }

    public static bool ShouldShowPasswordChangePanel(Associado? user)
    {
        return user != null && user.Senha == "avaliacao";
    }

    public static bool ShouldShowMainContent(Associado? user)
    {
        return user != null && user.Senha != "avaliacao";
    }

    public static bool ShouldShowQuickAccess(Associado? user)
    {
        return ShouldShowMainContent(user);
    }

    public static bool ShouldShowEvaluations(Associado? user)
    {
        return ShouldShowMainContent(user);
    }

    public static bool ShouldShowMentorship(Associado? user)
    {
        return ShouldShowMainContent(user);
    }

    public static bool ShouldShowMentorActions(List<Associado> mentorados)
    {
        return mentorados != null && mentorados.Any();
    }

    public static bool ShouldShowMentoredActions()
    {
        return true;
    }

    public static bool IsUserActive(Associado? user)
    {
        return user != null && user.Ativo;
    }

    public static bool HasPermissionLevel(Associado? user, int requiredLevel)
    {
        return user != null && user.IdPerfil >= requiredLevel;
    }

    public static bool CanManageProjects(List<dynamic> projectsAsManager)
    {
        return projectsAsManager != null && projectsAsManager.Any();
    }

    public static bool CanManageInternalFronts(List<dynamic> internalFrontsAsLeader)
    {
        return internalFrontsAsLeader != null && internalFrontsAsLeader.Any();
    }

    public static string GetVisibilityClass(bool isVisible)
    {
        return isVisible ? "" : "d-none";
    }

    public static string GetVisibilityStyle(bool isVisible)
    {
        return isVisible ? "" : "display: none;";
    }
}