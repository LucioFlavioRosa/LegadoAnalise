using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Perfis.Common;

public static class PerfisMapper
{
    public static PerfilDto ToDto(Perfil entity)
    {
        return new PerfilDto
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Ativo = entity.Ativo,
            DHC = entity.DHC
        };
    }

    public static Perfil ToEntity(PerfilDto dto)
    {
        return new Perfil
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Ativo = dto.Ativo,
            DHC = dto.DHC ?? DateTime.Now
        };
    }

    public static PerfilListItemDto ToListItemDto(Perfil entity)
    {
        return new PerfilListItemDto
        {
            Id = entity.Id,
            Nome = entity.Nome,
            StatusTexto = StatusHelper.GetStatusText(entity.Ativo),
            PodeInativar = entity.Ativo
        };
    }

    public static List<PerfilDto> ToDtoList(IEnumerable<Perfil> entities)
    {
        return entities.Select(ToDto).ToList();
    }

    public static List<PerfilListItemDto> ToListItemDtoList(IEnumerable<Perfil> entities)
    {
        return entities.Select(ToListItemDto).ToList();
    }
}