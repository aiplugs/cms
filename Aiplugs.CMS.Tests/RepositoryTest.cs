using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Data;
using Aiplugs.CMS.Core.Query;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Aiplugs.CMS.Tests
{
    public class RepositoryTest
    {
        private async Task All<TRepository>(Func<TRepository,Task> action)
        {
            using (var testdb = new TestDb())
            {
                foreach(var db in testdb.DBs)
                {
                    await action(db.Resolve<TRepository>());
                }
            }
        }
        private async Task All<TRepository, ERepository>(Func<TRepository, ERepository, Task> action)
        {
            using (var testdb = new TestDb())
            {
                foreach(var db in testdb.DBs)
                {
                    await action(db.Resolve<TRepository>(), db.Resolve<ERepository>());
                }
            }
        }

        [Fact]
        public async Task Data_AddAsync()
        {
            await All<IDataRepository>(async repo => 
            {
                var id = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }), "tester");

                Assert.True(id > 0);
            });
        }

        [Fact]
        public async Task Data_LookupAsync()
        {
            await All<IDataRepository>(async repo => 
            {
                var id = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }), "tester");
                
                Assert.True(id > 0);
    
                var item = await repo.LookupAsync(id);

                Assert.Equal("Hello, World!", item.Data["Text"]);
            });
        }

        [Fact]
        public async Task Data_UpdateAsync()
        {
            await All<IDataRepository>(async repo => 
            {
                var id = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }), "tester1");
                
                Assert.True(id > 0);
    
                await repo.UpdateAsync(id, JObject.FromObject(new { Text = "Hello, World!!" }), "tester2");
                await repo.UpdateAsync(id, JObject.FromObject(new { Text = "Hello, World!!!" }), "tester3");

                var item = await repo.LookupAsync(id);
                
                Assert.Equal("Hello, World!!!", item.Data["Text"]);
                Assert.Equal("tester3", item.UpdatedBy);
            });
        }

        [Fact]
        public async Task Data_GetHistoryAsync()
        {
            await All<IDataRepository>(async repo => 
            {
                var id = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }), "tester1");
                
                Assert.True(id > 0);

                await repo.UpdateAsync(id, JObject.FromObject(new { Text = "Hello, World!!" }), "tester2");
                await repo.UpdateAsync(id, JObject.FromObject(new { Text = "Hello, World!!!" }), "tester3");

                var history = (await repo.GetHistoryAsync(id, null, 100)).ToArray();

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
                var id = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }), "tester1");
                
                Assert.True(id > 0);

                await repo.UpdateAsync(id, JObject.FromObject(new { Text = "Hello, World!!" }), "tester2");
                await repo.UpdateAsync(id, JObject.FromObject(new { Text = "Hello, World!!!" }), "tester3");

                var events = (await repo.GetEventsAsync("test", pivot, 100)).ToArray();

                Assert.Equal(3, events.Length);
                Assert.True(events[0] is CreateEvent);
                Assert.True(events[1] is UpdateEvent);
                Assert.True(events[2] is UpdateEvent);
            });
        }

        [Fact]
        public async Task Data_SetStatusAsync()
        {
            await All<IDataRepository>(async repo => 
            {
                var id = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }), "tester");
                
                Assert.True(id > 0);

                await repo.SetStatusAsync(id, false);

                var item = await repo.LookupAsync(id);

                Assert.Equal(false, item.IsValid);
            });
        }

        [Fact]
        public async Task Data_RemoveAsync()
        {
            await All<IDataRepository>(async repo => 
            {
                var id = await repo.AddAsync("test", JObject.FromObject(new { Text = "Hello, World!" }), "tester");
                
                Assert.True(id > 0);

                var ex = await Xunit.Record.ExceptionAsync(async() => await repo.RemoveAsync(id));

                Assert.Null(ex);

                var item = await repo.LookupAsync(id);

                Assert.Null(item);
            });
        }

        [Fact]
        public async Task Data_GetAsync()
        {
            await All<IDataRepository>(async repo => 
            {
                var id1 = await repo.AddAsync("test", JObject.FromObject(new { Text = "11" }), "tester");
                var id2 = await repo.AddAsync("test", JObject.FromObject(new { Text = "12" }), "tester");
                var id3 = await repo.AddAsync("test", JObject.FromObject(new { Text = "13" }), "tester");

                Assert.Equal(3, (await repo.GetAsync("test", null, true, null, 100)).Count());
                Assert.Equal(3, (await repo.GetAsync("test", "1", true, null, 100)).Count());
                Assert.Equal(1, (await repo.GetAsync("test", "2", true, null, 100)).Count());
                Assert.Equal(1, (await repo.GetAsync("test", "3", true, null, 100)).Count());
                Assert.Equal(2, (await repo.GetAsync("test", "1", false, id1, 100)).Count());
                Assert.Equal(1, (await repo.GetAsync("test", "1", false, id2, 100)).Count());

                var asc = (await repo.GetAsync("test", null, false, null, 100)).ToArray();
                Assert.Equal("11", (string)asc[0].Data["Text"]);
                Assert.Equal("12", (string)asc[1].Data["Text"]);
                Assert.Equal("13", (string)asc[2].Data["Text"]);
                
                var desc = (await repo.GetAsync("test", null, true, null, 100)).ToArray();
                Assert.Equal("13", (string)desc[0].Data["Text"]);
                Assert.Equal("12", (string)desc[1].Data["Text"]);
                Assert.Equal("11", (string)desc[2].Data["Text"]);
            });
        }

        [Fact]
        public async Task Data_QueryAsync()
        {
            await All<IDataRepository>(async repo => 
            {
                var id1 = await repo.AddAsync("test", JObject.FromObject(new { Text = "11" }), "tester");
                var id2 = await repo.AddAsync("test", JObject.FromObject(new { Text = "12" }), "tester");
                var id3 = await repo.AddAsync("test", JObject.FromObject(new { Text = "13" }), "tester");

                Assert.Equal(3, (await repo.QueryAsync("test", QParser.Parse("$.Text like '%'").Expression, true, null, 100)).Count());
                Assert.Equal(3, (await repo.QueryAsync("test", QParser.Parse("$.Text like '1%'").Expression, true, null, 100)).Count());
                Assert.Equal(1, (await repo.QueryAsync("test", QParser.Parse("$.Text like '%2'").Expression, true, null, 100)).Count());
                Assert.Equal(1, (await repo.QueryAsync("test", QParser.Parse("$.Text like '%3'").Expression, true, null, 100)).Count());
                Assert.Equal(2, (await repo.QueryAsync("test", QParser.Parse("$.Text like '1%'").Expression, false, id1, 100)).Count());
                Assert.Equal(1, (await repo.QueryAsync("test", QParser.Parse("$.Text like '1%'").Expression, false, id2, 100)).Count());

                var asc = (await repo.QueryAsync("test", QParser.Parse("$.Text like '%'").Expression, false, null, 100)).ToArray();
                Assert.Equal("11", (string)asc[0].Data["Text"]);
                Assert.Equal("12", (string)asc[1].Data["Text"]);
                Assert.Equal("13", (string)asc[2].Data["Text"]);
                
                var desc = (await repo.QueryAsync("test", QParser.Parse("$.Text like '%'").Expression, true, null, 100)).ToArray();
                Assert.Equal("13", (string)desc[0].Data["Text"]);
                Assert.Equal("12", (string)desc[1].Data["Text"]);
                Assert.Equal("11", (string)desc[2].Data["Text"]);
            });
        }

        [Fact]
        public async Task Folder_AddAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var id = await repo.AddAsync("/Test/");

                Assert.True(id > 0);

                var ex = await Xunit.Record.ExceptionAsync(async () => await repo.AddAsync("/Test/"));

                Assert.NotNull(ex);
            });
        }

        [Fact]
        public async Task Folder_LookupAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var id = await repo.AddAsync("/Test/");

                Assert.True(id > 0);

                var folder = await repo.LookupAsync(id);

                Assert.NotNull(folder);
                Assert.Equal("/Test/", folder.Path);
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
                var id = await repo.AddAsync("/Test/");

                Assert.True(id > 0);

                var ex = await Xunit.Record.ExceptionAsync(async () => await repo.RemoveAsync(id));

                Assert.Null(ex);

                var folder = await repo.LookupAsync(id);

                Assert.Null(folder);
            });
        }

        [Fact]
        public async Task Folder_UpdateAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var id = await repo.AddAsync("/Test/");

                Assert.True(id > 0);

                await repo.UpdateAsync(id, "/Test1/");

                var folder = await repo.LookupAsync(id);

                Assert.Equal("/Test1/", folder.Path);
            });
        }

        [Fact]
        public async Task Folder_GetAsync()
        {
            await All<IFolderRepository>(async repo =>
            {
                var id = await repo.AddAsync("/Test/");

                Assert.True(id > 0);

                var folder = await repo.GetAsync("/Test/");

                Assert.NotNull(folder);
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
                var id = await folders.AddAsync("/Test/");

                await files.AddAsync(id, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);

                var ex = await Xunit.Record.ExceptionAsync(async () => 
                {
                    await files.AddAsync(id, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);
                });

                Assert.NotNull(ex);
            });
        }

        [Fact]
        public async Task File_LookupAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var folderId = await folders.AddAsync("/Test/");

                var id = await files.AddAsync(folderId, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);

                var file = await files.LookupAsync(id);

                Assert.NotNull(file);
                Assert.Equal("TestFile", file.Name);
            });
        }

        [Fact]
        public async Task File_UpdateAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var folderId = await folders.AddAsync("/Test/");

                var id = await files.AddAsync(folderId, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);

                await files.UpdateAsync(id, folderId, "TestFile1", "text/plain", 100, "tester", DateTime.UtcNow);

                var file = await files.LookupAsync(id);
                
                Assert.NotNull(file);
                Assert.Equal("TestFile1", file.Name);
            });
        }

        [Fact]
        public async Task File_RemoveAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var folderId = await folders.AddAsync("/Test/");

                var id = await files.AddAsync(folderId, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);

                await files.UpdateAsync(id, folderId, "TestFile1", "text/plain", 100, "tester", DateTime.UtcNow);
                
                var ex = await Xunit.Record.ExceptionAsync(async() => await files.RemoveAsync(id));
               
                Assert.Null(ex);
                
                var file = await files.LookupAsync(id);
                
                Assert.Null(file);
            });
        }

        [Fact]
        public async Task File_FindChildAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var id = await folders.AddAsync("/Test/");

                await files.AddAsync(id, "TestFile", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);
                
                var file = await files.FindChildAsync(id, "TestFile");
                
                Assert.NotNull(file);
            });
        }

        [Fact]
        public async Task File_FindChildrenAsync()
        {
            await All<IFolderRepository, IFileRepository>(async (folders, files) =>
            {
                var id = await folders.AddAsync("/Test/");

                var id1 = await files.AddAsync(id, "TestFile1", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);
                var id2 = await files.AddAsync(id, "TestFile2", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);
                var id3 = await files.AddAsync(id, "TestFile3", Path.GetTempFileName(), "text/plain", 100, "tester", DateTime.UtcNow);
                
                Assert.Equal(3, (await files.GetChildrenAsync(id, null, 100)).Count());
                Assert.Equal(2, (await files.GetChildrenAsync(id, null, 2)).Count());
                Assert.Equal(2, (await files.GetChildrenAsync(id, id1, 100)).Count());
                Assert.Equal(1, (await files.GetChildrenAsync(id, id2, 100)).Count());
            });
        }
    }
}