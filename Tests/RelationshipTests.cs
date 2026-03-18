namespace Tests;

[TestClass]
public class RelationshipTests : IntegrationTestBase
{
    [TestMethod]
    public void DeletePost_CascadeDeleteComments()
    {
        // Given
        var post = DataGenerator.GetPostFaker().Generate();
        _context.Posts.Add(post);
        _context.SaveChanges();

        var comments = DataGenerator.GetCommentFaker(post.Id).Generate(3);
        _context.Comments.AddRange(comments);
        _context.SaveChanges();

        // When
        _repository.DeletePost(post.Id);

        // Then
        var remainingComments = _repository
            .GetCommentsByPostId(post.Id)
            .ToList();

        Assert.AreEqual(0, remainingComments.Count);
    }
}