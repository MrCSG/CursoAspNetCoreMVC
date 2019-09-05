/* The MarkDoneAsync() method returns false if it's passed an ID that
doesn't exist
The MarkDoneAsync() method returns true when it makes a valid
item as complete
The GetIncompleteItemsAsync() method returns only the items
owned by a particular user */

using System;
using System.Threading.Tasks;
using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AspNetCoreTodo.UnitTest
{
    public class TodoItemServiceTest
    {
        [Fact]
        public async Task AddNewItemAsIncompleteWithDueDate()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "Test_AddNewItem").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new ApplicationUser
                {
                    Id = "fake-000",
                    UserName = "fake@example.com"
                };

                var todoItem = new TodoItem
                {
                    Title = "Testing?"
                };

                await service.AddItemAsync(todoItem, fakeUser);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var itemsInDatabase = await context.Items.CountAsync();
                Assert.Equal(1, itemsInDatabase);
                
                var item = await context.Items.FirstAsync();
                Assert.Equal("Testing?", item.Title);
                Assert.Equal(false, item.IsDone);

                var difference = DateTimeOffset.Now.AddDays(3) - item.DueAt;
                Assert.True(difference < TimeSpan.FromSeconds(1));
            }
        }
        //The MarkDoneAsync() method returns false if it's passed an ID that doesn't exist
        [Fact]
        public async Task GivenAnInexistentID_MarkDoneAsync_ShouldReturnFalse()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "Test_MarkDone_Failed").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new ApplicationUser{
                    Id = "fake-000",
                    UserName = "fake@test.com"
                };

                var fakeTodoItem = new TodoItem{
                    Title = "TestItem_MarkDoneFailed"
                };

                await service.AddItemAsync(fakeTodoItem, fakeUser);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var items = await context.Items.ToArrayAsync();
                var fakeUser1 = new ApplicationUser{
                    Id = "fake-001",
                    UserName = "fake1@test.com"
                };

                Assert.Equal(1, items.Length);
                Assert.Equal("TestItem_MarkDoneFailed", items[0].Title);
                Assert.Equal(false, items[0].IsDone);
                Assert.Equal(false, await service.MarkDoneAsync(items[0].Id, fakeUser1));
            }
        }
        //The MarkDoneAsync() method returns true when it makes a valid item as complete
        [Fact]
        public async Task GivenAValidItem_MarkDoneAsync_ShouldReturnTrue()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "Test_MarkDone_Passed").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new ApplicationUser{
                    Id = "fake-000",
                    UserName = "fake@test.com"
                };

                var fakeTodoItem = new TodoItem{
                    Title = "TestItem_MarkDonePassed"
                };

                await service.AddItemAsync(fakeTodoItem, fakeUser);
            }
        
            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var items = await context.Items.ToArrayAsync();
                var fakeUser = new ApplicationUser{
                    Id = "fake-000",
                    UserName = "fake@test.com"
                };

                Assert.Equal(1, items.Length);
                Assert.Equal("TestItem_MarkDonePassed", items[0].Title);
                Assert.Equal(false, items[0].IsDone);
                Assert.Equal(true, await service.MarkDoneAsync(items[0].Id, fakeUser));
            }
        }
        //The GetIncompleteItemsAsync() method returns only the items owned by a particular user
        [Fact]
        public async Task GivenAnUser_GetIncompleteItemsAsync_ShouldReturnItemsOwnedByUser()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "Test_GetIncompleteItems").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new ApplicationUser{
                    Id = "fake-000",
                    UserName = "fake@test.com"
                };

                var fakeTodoItem = new TodoItem{
                    Title = "TestItem_GetIncompleteItems"
                };

                await service.AddItemAsync(fakeTodoItem, fakeUser);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var fakeUser = new ApplicationUser{
                    Id = "fake-000",
                    UserName = "fake@test.com"
                };
                var items = await service.GetIncompleteItemsAsync(fakeUser);

                Assert.Equal(1, items.Length);
                Assert.Equal("TestItem_GetIncompleteItems", items[0].Title);
                Assert.Equal(false, items[0].IsDone);
                Assert.Equal("fake-000", items[0].UserId);
            }
        }
    }
}