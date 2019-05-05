using Aiplugs.CMS.Data.Repositories;
using Aiplugs.CMS.Query;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Aiplugs.CMS.Tests
{
    public class RepositoryTest
    {
        #region helpers
        private async Task All<TRepository>(Func<TRepository, Task> action)
        {
            using (var testdb = new TestDb())
            {
                foreach (var db in testdb.DBs)
                {
                    await action(db.Resolve<TRepository>());
                }
            }
        }
        private async Task All<TRepository, ERepository>(Func<TRepository, ERepository, Task> action)
        {
            using (var testdb = new TestDb())
            {
                foreach (var db in testdb.DBs)
                {
                    await action(db.Resolve<TRepository>(), db.Resolve<ERepository>());
                }
            }
        }
        #endregion

        [Fact]
        public async Task Data_AddAsync()
        {
            await All<IDataRepository>(async repo =>
            {
                var datum = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }).ToString(), "tester", DateTimeOffset.UtcNow);

                Assert.NotNull(datum);
            });
        }

        [Fact]
        public async Task Data_LookupAsync()
        {
            await All<IDataRepository>(async repo =>
            {
                var datum = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }).ToString(), "tester", DateTimeOffset.UtcNow);

                Assert.NotNull(datum);

                var item = await repo.LookupAsync(datum.Id);

                Assert.Equal("Hello, World!", item.Data("Text"));
            });
        }

        [Fact]
        public async Task Data_UpdateAsync()
        {
            await All<IDataRepository>(async repo =>
            {
                var datum = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }).ToString(), "tester1", DateTimeOffset.UtcNow);

                Assert.NotNull(datum);

                await repo.UpdateDataAsync(datum.Id, JObject.FromObject(new { Text = "Hello, World!!" }).ToString(), datum.CurrentId, "tester2", DateTimeOffset.UtcNow);

                var item = await repo.LookupAsync(datum.Id);

                Assert.Equal("Hello, World!!", item.Data("Text"));
                Assert.Equal("tester2", item.UpdatedBy);
            });
        }

        [Fact]
        public async Task Data_GetHistoryAsync()
        {
            await All<IDataRepository>(async repo =>
            {
                var datum = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }).ToString(), "tester1", DateTimeOffset.UtcNow);

                Assert.NotNull(datum);

                await repo.UpdateDataAsync(datum.Id, JObject.FromObject(new { Text = "Hello, World!!" }).ToString(), datum.CurrentId, "tester2", DateTimeOffset.UtcNow);

                var item = await repo.LookupAsync(datum.Id);

                await repo.UpdateDataAsync(datum.Id, JObject.FromObject(new { Text = "Hello, World!!!" }).ToString(), item.CurrentId, "tester3", DateTimeOffset.UtcNow);

                var history = (await repo.GetHistoryAsync(datum.Id, null, 100)).ToArray();

                Assert.Equal(3, history.Length);
                Assert.Equal("tester3", history[0].CreatedBy);
                Assert.Equal("tester2", history[1].CreatedBy);
                Assert.Equal("tester1", history[2].CreatedBy);
            });
        }

        [Fact]
        public async Task Data_GetEventsAsync()
        {
            var pivot = DateTime.UtcNow;

            await Task.Delay(TimeSpan.FromSeconds(1));

            await All<IDataRepository>(async repo =>
            {
                var datum = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }).ToString(), "tester1", DateTimeOffset.UtcNow);

                Assert.NotNull(datum);

                await repo.UpdateDataAsync(datum.Id, JObject.FromObject(new { Text = "Hello, World!!" }).ToString(), datum.CurrentId, "tester2", DateTimeOffset.UtcNow);

                var events = (await repo.GetEventsAsync("test", pivot, null, 100)).ToArray();

                Assert.Equal(1, events.Length);
                Assert.True(events[0] is CreateEvent);
            });
        }

        [Fact]
        public async Task Data_GetRecordThenAsync()
        {
            await All<IDataRepository>(async repo =>
            {
                var datum = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }).ToString(), "tester1", DateTimeOffset.UtcNow);

                Assert.NotNull(datum);

                await repo.UpdateDataAsync(datum.Id, JObject.FromObject(new { Text = "Hello, World!!" }).ToString(), datum.CurrentId, "tester2", DateTimeOffset.UtcNow);

                var pivot = DateTimeOffset.UtcNow;

                var item = await repo.LookupAsync(datum.Id);

                await repo.UpdateDataAsync(datum.Id, JObject.FromObject(new { Text = "Hello, World!!!" }).ToString(), item.CurrentId, "tester3", DateTimeOffset.UtcNow);

                var record = await repo.GetRecordThenAsync(datum.Id, pivot);

                Assert.NotNull(record);
                Assert.Equal("tester2", record.CreatedBy);
                Assert.Equal("Hello, World!!", record.Data("Text"));
            });
        }

        [Fact]
        public async Task Data_SetStatusAsync()
        {
            await All<IDataRepository>(async repo =>
            {
                var datum = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }).ToString(), "tester", DateTimeOffset.UtcNow);

                Assert.NotNull(datum);

                await repo.UpdateStatusAsync(datum.Id, false, datum.CurrentId);

                var item = await repo.LookupAsync(datum.Id);

                Assert.Equal(false, item.IsValid);
            });
        }

        [Fact]
        public async Task Data_RemoveAsync()
        {
            await All<IDataRepository>(async repo =>
            {
                var datum = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }).ToString(), "tester", DateTimeOffset.UtcNow);

                Assert.NotNull(datum);

                var ex = await Xunit.Record.ExceptionAsync(async () => await repo.RemoveAsync(datum.Id));

                Assert.Null(ex);

                var item = await repo.LookupAsync(datum.Id);

                Assert.Null(item);
            });
        }

        [Fact]
        public async Task Data_GetAsync()
        {
            await All<IDataRepository>(async repo =>
            {
                var d1 = await repo.AddAsync("test", JObject.FromObject(new { Text = "11" }).ToString(), "tester", DateTimeOffset.UtcNow);
                var d2 = await repo.AddAsync("test", JObject.FromObject(new { Text = "12" }).ToString(), "tester", DateTimeOffset.UtcNow);
                var d3 = await repo.AddAsync("test", JObject.FromObject(new { Text = "13" }).ToString(), "tester", DateTimeOffset.UtcNow);

                Assert.Equal(3, (await repo.GetAsync("test", null, true, null, 100)).Count());
                Assert.Equal(3, (await repo.GetAsync("test", "1", true, null, 100)).Count());
                Assert.Equal(1, (await repo.GetAsync("test", "2", true, null, 100)).Count());
                Assert.Equal(1, (await repo.GetAsync("test", "3", true, null, 100)).Count());
                Assert.Equal(2, (await repo.GetAsync("test", "1", false, d1.Id, 100)).Count());
                Assert.Equal(1, (await repo.GetAsync("test", "1", false, d2.Id, 100)).Count());

                var asc = (await repo.GetAsync("test", null, false, null, 100)).ToArray();
                Assert.Equal("11", asc[0].Data("Text"));
                Assert.Equal("12", asc[1].Data("Text"));
                Assert.Equal("13", asc[2].Data("Text"));

                var desc = (await repo.GetAsync("test", null, true, null, 100)).ToArray();
                Assert.Equal("13", desc[0].Data("Text"));
                Assert.Equal("12", desc[1].Data("Text"));
                Assert.Equal("11", desc[2].Data("Text"));
            });
        }

        [Fact]
        public async Task Data_QueryAsync()
        {
            await All<IDataRepository>(async repo =>
            {
                var d1 = await repo.AddAsync("test", JObject.FromObject(new { Text = "11" }).ToString(), "tester", DateTimeOffset.UtcNow);
                var d2 = await repo.AddAsync("test", JObject.FromObject(new { Text = "12" }).ToString(), "tester", DateTimeOffset.UtcNow);
                var d3 = await repo.AddAsync("test", JObject.FromObject(new { Text = "13" }).ToString(), "tester", DateTimeOffset.UtcNow);

                var asc = (await repo.QueryAsync("test", QParser.Parse("$.Text like '%'").Expression, false, null, 100)).ToArray();
                Assert.Equal("11", asc[0].Data("Text"));
                Assert.Equal("12", asc[1].Data("Text"));
                Assert.Equal("13", asc[2].Data("Text"));

                var desc = (await repo.QueryAsync("test", QParser.Parse("$.Text like '%'").Expression, true, null, 100)).ToArray();
                Assert.Equal("13", desc[0].Data("Text"));
                Assert.Equal("12", desc[1].Data("Text"));
                Assert.Equal("11", desc[2].Data("Text"));

                Assert.Equal(3, (await repo.QueryAsync("test", QParser.Parse("$.Text like '%'").Expression, true, null, 100)).Count());
                Assert.Equal(3, (await repo.QueryAsync("test", QParser.Parse("$.Text like '1%'").Expression, true, null, 100)).Count());
                Assert.Equal(1, (await repo.QueryAsync("test", QParser.Parse("$.Text like '%2'").Expression, true, null, 100)).Count());
                Assert.Equal(1, (await repo.QueryAsync("test", QParser.Parse("$.Text like '%3'").Expression, true, null, 100)).Count());
                Assert.Equal(2, (await repo.QueryAsync("test", QParser.Parse("$.Text like '1%'").Expression, false, d1.Id, 100)).Count());
                Assert.Equal(1, (await repo.QueryAsync("test", QParser.Parse("$.Text like '1%'").Expression, false, d2.Id, 100)).Count());
            });
        }

        [Fact]
        public async Task Folder_AddAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var folder = await repo.AddAsync("/Test/");

                Assert.NotNull(folder);

                var ex = await Xunit.Record.ExceptionAsync(async () => await repo.AddAsync("/Test/"));

                Assert.NotNull(ex);
            });
        }

        [Fact]
        public async Task Folder_LookupAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var folder = await repo.AddAsync("/Test/");

                Assert.NotNull(folder);

                var lookuped = await repo.LookupAsync(folder.Id);

                Assert.NotNull(lookuped);
                Assert.Equal("/Test/", lookuped.Path);
            });
        }

        [Fact]
        public async Task Folder_LookupHomeAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var folder = await repo.LookupHomeAsync();

                Assert.NotNull(folder);
                Assert.Equal("/", folder.Path);
            });
        }

        [Fact]
        public async Task Folder_RemoveAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var folder = await repo.AddAsync("/Test/");

                Assert.NotNull(folder);

                var ex = await Xunit.Record.ExceptionAsync(async () => await repo.RemoveAsync(folder.Id));

                Assert.Null(ex);

                var lookuped = await repo.LookupAsync(folder.Id);

                Assert.Null(lookuped);
            });
        }

        [Fact]
        public async Task Folder_UpdateAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var folder = await repo.AddAsync("/Test/");

                Assert.NotNull(folder);

                await repo.UpdateAsync(folder.Id, "/Test1/");

                var lookuped = await repo.LookupAsync(folder.Id);

                Assert.Equal("/Test1/", lookuped.Path);
            });
        }

        [Fact]
        public async Task Folder_GetAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var folder = await repo.AddAsync("/Test/");

                Assert.NotNull(folder);

                var got = await repo.GetAsync("/Test/");

                Assert.NotNull(got);
            });
        }

        [Fact]
        public async Task Folder_GetChildrenAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                await repo.AddAsync("/Test/");
                await repo.AddAsync("/Test/Test1/");
                await repo.AddAsync("/Test/Test1/Test11/");
                await repo.AddAsync("/Test/Test1/Test12/");
                await repo.AddAsync("/Test/Test2/");
                await repo.AddAsync("/Test/Test2/Test21/");
                await repo.AddAsync("/Test/Test2/Test22/");
                await repo.AddAsync("/Test/Test3/");

                var folders = (await repo.GetChildrenAsync("/Test/", null, 100)).ToArray();

                Assert.Equal(3, folders.Length);
                Assert.Equal("/Test/Test1/", folders[0].Path);
                Assert.Equal("/Test/Test2/", folders[1].Path);
                Assert.Equal("/Test/Test3/", folders[2].Path);
            });
        }

        [Fact]
        public async Task File_AddAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var folder = await folders.AddAsync("/Test/");

                await files.AddAsync(folder.Id, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);

                var ex = await Xunit.Record.ExceptionAsync(async () =>
                {
                    await files.AddAsync(folder.Id, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);
                });

                Assert.NotNull(ex);
            });
        }

        [Fact]
        public async Task File_LookupAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var folder = await folders.AddAsync("/Test/");

                var file = await files.AddAsync(folder.Id, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);

                var lookuped = await files.LookupAsync(file.Id);

                Assert.NotNull(lookuped);
                Assert.Equal("TestFile", lookuped.Name);
            });
        }

        [Fact]
        public async Task File_UpdateAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var folder = await folders.AddAsync("/Test/");

                var file = await files.AddAsync(folder.Id, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);

                await files.UpdatePathAsync(file.Id, folder.Id, "TestFile1", "tester", DateTime.UtcNow);
                await files.UpdateMetaAsync(file.Id, "application/json", 100, "tester", DateTime.UtcNow);

                var lookuped = await files.LookupAsync(file.Id);

                Assert.NotNull(lookuped);
                Assert.Equal("TestFile1", lookuped.Name);
                Assert.Equal("application/json", lookuped.ContentType);
            });
        }

        [Fact]
        public async Task File_RemoveAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var folder = await folders.AddAsync("/Test/");

                var file = await files.AddAsync(folder.Id, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);
                
                var ex = await Xunit.Record.ExceptionAsync(async () => await files.RemoveAsync(file.Id));

                Assert.Null(ex);

                var lookuped = await files.LookupAsync(file.Id);

                Assert.Null(lookuped);
            });
        }

        [Fact]
        public async Task File_FindChildAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var folder = await folders.AddAsync("/Test/");

                await files.AddAsync(folder.Id, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);

                var file = await files.FindChildAsync(folder.Id, "TestFile");

                Assert.NotNull(file);
            });
        }

        [Fact]
        public async Task File_FindChildrenAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var folder = await folders.AddAsync("/Test/");

                var f1 = await files.AddAsync(folder.Id, "TestFile1", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);
                var f2 = await files.AddAsync(folder.Id, "TestFile2", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);
                var f3 = await files.AddAsync(folder.Id, "TestFile3", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);

                Assert.Equal(3, (await files.GetChildrenAsync(folder.Id, null, null, 100)).Count());
                Assert.Equal(2, (await files.GetChildrenAsync(folder.Id, null, null, 2)).Count());
                Assert.Equal(2, (await files.GetChildrenAsync(folder.Id, null, f1.Id, 100)).Count());
                Assert.Equal(1, (await files.GetChildrenAsync(folder.Id, null, f2.Id, 100)).Count());
            });
        }
    }
}