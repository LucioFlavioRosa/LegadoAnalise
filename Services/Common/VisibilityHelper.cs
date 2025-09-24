using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Common;

public static class VisibilityHelper
{
    public static bool ShouldShowAdminMenu(int userProfileId)
    {
        return userProfileId > 2;
    }

    public static bool ShouldShowEvaluationOptions(int userProfileId)
    {
        return userProfileId > 1;
    }

    public static bool ShouldShowPasswordChangePanel(Associado? user)
    {
        return user != null && IsDefaultPassword(user.Senha);
    }

    public static bool ShouldShowMainContent(Associado? user)
    {
        return user != null && !IsDefaultPassword(user.Senha);
    }

    public static bool ShouldShowQuickAccess(Associado? user)
    {
        return ShouldShowMainContent(user);
    }

    public static bool ShouldShowEvaluationAccess(Associado? user)
    {
        return ShouldShowMainContent(user);
    }

    public static bool ShouldShowMentorshipSection(List<Associado> mentorados)
    {
        return mentorados != null && mentorados.Any();
    }

    public static bool ShouldShowInternalFrontButton(bool isInternalFrontLeader)
    {
        return isInternalFrontLeader;
    }

    public static bool ShouldShowProjectManagementButton(bool hasProjects)
    {
        return hasProjects;
    }

    public static bool IsUserActive(Associado? user)
    {
        return user != null && user.Ativo;
    }

    public static bool IsUserAuthenticated(Associado? user)
    {
        return user != null && user.Id > 0;
    }

    public static bool HasRequiredProfile(int userProfileId, int requiredProfileId)
    {
        return userProfileId >= requiredProfileId;
    }

    public static bool ShouldShowMenuItem(MenuItem menuItem, int userProfileId)
    {
        return menuItem.IsVisible && ShouldShowAdminMenu(userProfileId);
    }

    public static bool ShouldShowQuickAccessItem(QuickAccessItem item, int userProfileId)
    {
        return item.IsVisible;
    }

    public static bool ShouldShowEvaluationItem(EvaluationMenuItem item, int userProfileId)
    {
        if (!item.IsVisible)
            return false;

        var restrictedItems = new[] { "cegas", "gestor", "feedback", "resultadoLideranca" };
        if (restrictedItems.Contains(item.Id))
        {
            return ShouldShowEvaluationOptions(userProfileId);
        }

        return true;
    }

    public static bool ShouldShowMentorshipItem(MentorshipMenuItem item, bool hasMentorados)
    {
        if (!item.IsVisible)
            return false;

        if (item.IsMentorSection)
        {
            return hasMentorados;
        }

        return true;
    }

    public static string GetVisibilityClass(bool isVisible)
    {
        return isVisible ? "" : "d-none";
    }

    public static string GetVisibilityStyle(bool isVisible)
    {
        return isVisible ? "" : "display: none;";
    }

    private static bool IsDefaultPassword(string? password)
    {
        return string.Equals(password?.Trim(), "avaliacao", StringComparison.OrdinalIgnoreCase);
    }

    public static class ProfileLevels
    {
        public const int Basic = 1;
        public const int Intermediate = 2;
        public const int Advanced = 3;
        public const int Admin = 4;
    }

    public static class VisibilityRules
    {
        public static readonly Dictionary<string, int> MenuItemProfileRequirements = new()
        {
            { "associados", ProfileLevels.Advanced },
            { "cargos", ProfileLevels.Advanced },
            { "perfilAcesso", ProfileLevels.Advanced },
            { "clientes", ProfileLevels.Advanced },
            { "tiposProjetos", ProfileLevels.Advanced },
            { "complexidade", ProfileLevels.Advanced },
            { "eixo", ProfileLevels.Advanced },
            { "dimensoes", ProfileLevels.Advanced },
            { "subCompetencias", ProfileLevels.Advanced },
            { "competencias", ProfileLevels.Advanced },
            { "performance", ProfileLevels.Advanced },
            { "projetos", ProfileLevels.Advanced },
            { "envioAvaliacoes", ProfileLevels.Advanced }
        };

        public static readonly Dictionary<string, int> EvaluationItemProfileRequirements = new()
        {
            { "autoAvaliacao", ProfileLevels.Basic },
            { "evolucaoAssociado", ProfileLevels.Basic },
            { "avaliacoesLideranca", ProfileLevels.Basic },
            { "cegas", ProfileLevels.Intermediate },
            { "gestor", ProfileLevels.Intermediate },
            { "feedback", ProfileLevels.Intermediate },
            { "resultadoLideranca", ProfileLevels.Intermediate }
        };
    }
}