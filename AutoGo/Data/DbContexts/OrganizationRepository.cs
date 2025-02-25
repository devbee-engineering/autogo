using Agoda.IoC.Core;
using AutoGo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoGo.Data.DbContexts;

public interface IOrganizationRepository
{
    Task Add(Organization organization);
    Task<Organization> Get(int id);
}

[RegisterPerRequest]
public class OrganizationRepository(AppDbContext appDbContext) : IOrganizationRepository
{
    public async Task Add(Organization organization)
    {
        await appDbContext.Organizations.AddAsync(organization);
    }

    public async Task<Organization> Get(int id)
    {
        return await appDbContext.Organizations.Where(x => x.Id == id).FirstAsync();
    }
}
