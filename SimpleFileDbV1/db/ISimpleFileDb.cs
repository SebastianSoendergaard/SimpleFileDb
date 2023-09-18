﻿namespace SimpleFileDatabase;

public interface ISimpleFileDb
{
    public Task CreateAsync<Tdata>(Tdata data) where Tdata : class;
    public Task UpdateAsync<Tdata>(Tdata data) where Tdata : class;
    public Task<Tdata?> GetByIdAsync<Tdata>(object id) where Tdata : class;
    public Task<IEnumerable<Tdata>> GetAll<Tdata>() where Tdata : class;
    public Task DeleteByIdAsync<Tdata>(object id) where Tdata : class;
}
