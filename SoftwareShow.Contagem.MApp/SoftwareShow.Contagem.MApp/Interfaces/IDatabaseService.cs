using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Interfaces
{
    public interface IDatabaseService
    {
        Task<SQLiteAsyncConnection> GetConnectionAsync();
        Task InitializeDatabaseAsync();
        Task<int> InsertAsync<T>(T item) where T : new();
        Task<int> InsertAllAsync<T>(IEnumerable<T> items) where T : new();
        Task<int> UpdateAsync<T>(T item) where T : new();
        Task<int> DeleteAsync<T>(T item) where T : new();
        Task<int> DeleteAllAsync<T>() where T : new();
        Task<List<T>> GetAllAsync<T>() where T : new();
        Task<T> GetAsync<T>(object pk) where T : new();
        Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new();
        Task CloseAsync();
    }
}
