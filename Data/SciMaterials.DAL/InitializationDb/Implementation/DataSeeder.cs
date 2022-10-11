using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Properties;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.DAL.InitializationDb.Implementation
{
    internal static class DataSeeder
    {
        //data generation was carried out by https://generatedata.com

        /// <summary> Асинхронно выполняет транзакцию заполнения таблиц базы данных тестовыми данными. В случае ошибки транзакция не выполняется.</summary>
        /// <param name="db">Контекст базы данных.</param>
        /// <param name="cancel">Распространяет уведомление о том, что операции следует отменить. <see cref="CancellationToken"/> Значение по умолчанию: <value>default</value></param>
        /// <returns>Задача, которая представляет работу в очереди на выполнение в ThreadPool. См. <see cref="Task"/></returns>
        /// <exception cref="OperationCanceledException"></exception>
        internal static async Task SeedAsync(SciMaterialsContext db, CancellationToken cancel = default)
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancel).ConfigureAwait(false);

            if (!await db.Users.AnyAsync(cancel))
            {
                try
                {
                    await db.Users.AddRangeAsync(JsonConvert.DeserializeObject<List<User>>(Encoding.UTF8.GetString(Resources.Users))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Authors", e.Message);
                    throw;
                }
            }

            if (!await db.Authors.AnyAsync(cancel))
            {
                try
                {
                    await db.Authors.AddRangeAsync(JsonConvert.DeserializeObject<List<Author>>(Encoding.UTF8.GetString(Resources.Authors))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Authors", e.Message);
                    throw;
                }
            }
          
            if (!await db.ContentTypes.AnyAsync(cancel))
            {
                try
                {
                    await db.ContentTypes.AddRangeAsync(JsonConvert.DeserializeObject<List<ContentType>>(Encoding.UTF8.GetString(Resources.ContentTypes))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data ContentTypes", e.Message);
                    throw;
                }
            }

            if (!await db.Tags.AnyAsync(cancel))
            {
                try
                {
                    await db.Tags.AddRangeAsync(JsonConvert.DeserializeObject<List<Tag>>(Encoding.UTF8.GetString(Resources.Tags))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Tags", e.Message);
                    throw;
                }
            }

            if (!await db.Categories.AnyAsync(cancel))
            {
                try
                {
                    await db.Categories.AddRangeAsync(JsonConvert.DeserializeObject<List<Category>>(Encoding.UTF8.GetString(Resources.Categories))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Categories", e.Message);
                    throw;
                }
            }

            if (!await db.FileGroups.AnyAsync(cancel))
            {
                try
                {
                    await db.FileGroups.AddRangeAsync(JsonConvert.DeserializeObject<List<FileGroup>>(Encoding.UTF8.GetString(Resources.FileGroups))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data FileGroup", e.Message);
                    throw;
                }
            }

            if (!await db.Files.AnyAsync(cancel))
            {
                try
                {
                    await db.Files.AddRangeAsync(JsonConvert.DeserializeObject<List<File>>(Encoding.UTF8.GetString(Resources.Files))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Files", e.Message);
                    throw;
                }
            }

            if (!await db.Comments.AnyAsync(cancel))
            {
                try
                {
                    await db.Comments.AddRangeAsync(JsonConvert.DeserializeObject<List<Comment>>(Encoding.UTF8.GetString(Resources.Comments))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Comments", e.Message);
                    throw;
                }
            }

            if (!await db.Ratings.AnyAsync(cancel))
            {
                try
                {
                    await db.Ratings.AddRangeAsync(JsonConvert.DeserializeObject<List<Rating>>(Encoding.UTF8.GetString(Resources.Ratings))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Ratings", e.Message);
                    throw;
                }
            }

            await transaction.CommitAsync(cancel).ConfigureAwait(false);
        }
    }
}
