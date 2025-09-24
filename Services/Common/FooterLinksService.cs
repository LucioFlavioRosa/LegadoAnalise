using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Common;

public interface IFooterLinksService
{
    Task<List<FooterLink>> GetFooterLinksAsync();
    Task<List<FooterLink>> GetFooterLinksAsync(int? userId, int? userProfileId);
}

public class FooterLinksService : IFooterLinksService
{
    private readonly ITelemetryService _telemetryService;

    public FooterLinksService(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    public async Task<List<FooterLink>> GetFooterLinksAsync()
    {
        return await GetFooterLinksAsync(null, null);
    }

    public async Task<List<FooterLink>> GetFooterLinksAsync(int? userId, int? userProfileId)
    {
        try
        {
            var links = new List<FooterLink>
            {
                new FooterLink
                {
                    Name = "TikTok",
                    Url = "https://www.tiktok.com/@peersbr",
                    Icon = "https://img.icons8.com/?size=48&id=118640&format=png",
                    IsVisible = true,
                    Order = 1
                },
                new FooterLink
                {
                    Name = "Instagram",
                    Url = "https://www.instagram.com/peers_br",
                    Icon = "https://img.icons8.com/?size=48&id=Xy10Jcu1L2Su&format=png",
                    IsVisible = true,
                    Order = 2
                },
                new FooterLink
                {
                    Name = "LinkedIn",
                    Url = "https://pt.linkedin.com/company/peersbr",
                    Icon = "https://cdn-icons-png.flaticon.com/128/145/145807.png",
                    IsVisible = true,
                    Order = 3
                },
                new FooterLink
                {
                    Name = "Ouvidoria",
                    Url = "https://www.peers4you.com.br/ouvidoria",
                    Icon = "https://cdn-icons-png.flaticon.com/128/9314/9314332.png",
                    IsVisible = true,
                    Order = 4
                },
                new FooterLink
                {
                    Name = "Peers4you",
                    Url = "https://www.peers4you.com.br/",
                    Icon = "https://static.wixstatic.com/media/9b7b3f_37cdc40265e04f81816fb55efb8e3a2c~mv2.png/v1/fill/w_51,h_40,al_c,q_85,usm_0.66_1.00_0.01,enc_avif,quality_auto/Favicon-01.png",
                    IsVisible = true,
                    Order = 5
                },
                new FooterLink
                {
                    Name = "Capacita",
                    Url = "https://peerscapacita.learning.rocks/login",
                    Icon = "https://cdn-icons-png.flaticon.com/128/8991/8991706.png",
                    IsVisible = true,
                    Order = 6
                },
                new FooterLink
                {
                    Name = "Reembolso/CRM",
                    Url = "https://star1crm.starsoft.com.br/peers/1crm_login.asp",
                    Icon = "https://cdn-icons-png.flaticon.com/128/3808/3808310.png",
                    IsVisible = true,
                    Order = 7
                },
                new FooterLink
                {
                    Name = "Holerite",
                    Url = "https://portal.starsoft.com.br/login",
                    Icon = "https://cdn-icons-png.flaticon.com/128/1127/1127283.png",
                    IsVisible = true,
                    Order = 8
                },
                new FooterLink
                {
                    Name = "Peers Brain",
                    Url = "https://brain.peers.com.br",
                    Icon = "https://brain.peers.com.br/avatar/peers.webp",
                    IsVisible = true,
                    Order = 9
                },
                new FooterLink
                {
                    Name = "Onfly",
                    Url = "https://app.onfly.com/login",
                    Icon = "https://cdn-icons-png.flaticon.com/128/10256/10256883.png",
                    IsVisible = true,
                    Order = 10
                }
            };

            _telemetryService.TrackEvent("FooterLinksLoaded", new Dictionary<string, string>
            {
                { "LinkCount", links.Count.ToString() },
                { "UserId", userId?.ToString() ?? "Anonymous" }
            });

            return links.Where(l => l.IsVisible).OrderBy(l => l.Order).ToList();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetFooterLinksAsync" },
                { "Component", "FooterLinksService" }
            });
            return new List<FooterLink>();
        }
    }
}

public class FooterLink
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int Order { get; set; } = 0;
    public string Target { get; set; } = "_blank";
    public string CssClass { get; set; } = string.Empty;
}