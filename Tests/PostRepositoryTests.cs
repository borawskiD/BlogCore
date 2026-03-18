using BlogCore.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Tests;

[TestClass]
public class PostRepositoryTests : IntegrationTestBase
{

    [TestMethod]
    public void AddPost_ValidPost_IncreasesCountByOne()
    {
        //given
        var post = new Post
        {
            Author = "Tests IT",
            Content = "This is testing post"
        };
        var before = _repository.GetAllPosts().Count();
        //when
        _repository.AddPost(post);
        //then
        var after = _repository.GetAllPosts().Count();
        Assert.AreEqual(before + 1, after);
    }
    
    [TestMethod]
    [ExpectedException(typeof(DbUpdateException))]
    public void AddPost_InvalidPost_ThrowDbUpdateException()
    {
        //given
        var post = new Post
        {
            Author = "Tests IT",
            Content = null
        };
        //when
        _repository.AddPost(post);
        
    }

    [TestMethod]
    public void GetAllPosts_EmptyDb_ReturnsZero()
    {
        //when
        var posts = _repository.GetAllPosts().ToList();
        //then
        Assert.AreEqual(0, posts.Count);
    }

    [TestMethod]
    public void AddPost_LongContent_SavesCorrectly()
    {
        // given
        var post = DataGenerator.GetPostFaker().Generate();
        post.Content = new Bogus.Faker().Lorem.Paragraphs(5);

        // when
        _repository.AddPost(post);

        // then
        var posts = _repository.GetAllPosts().ToList();

        Assert.AreEqual(post.Content, posts.First().Content);
    }

    [TestMethod]
    public void AddPost_SpecialCharactersInAuthor_SavesCorrectly()
    {
        // given
        var post = DataGenerator.GetPostFaker().Generate();
        post.Content = "Zażółć Gęślą Jaźń 123 żźćąśńłóę";

        // when
        _repository.AddPost(post);

        // then
        var posts = _repository.GetAllPosts().ToList();

        Assert.AreEqual(post.Content, posts.First().Content);
    }

}