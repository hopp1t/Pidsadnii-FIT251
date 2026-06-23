using Xunit;
using task03;

namespace task03tests;

public class IteratorTests
{
    [Fact]
    public void GetEnumerator_ReturnsAllItemsInOrder()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);
        collection.Add(3);

        var result = new List<int>();
        foreach (var item in collection)
        {
            result.Add(item);
        }

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void GetReverseEnumerator_ReturnsItemsInReverseOrder()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);
        collection.Add(3);

        var result = collection.GetReverseEnumerator().ToList();

        Assert.Equal(new[] { 3, 2, 1 }, result);
    }

    [Fact]
    public void GetReverseEnumerator_EmptyCollection_ReturnsEmpty()
    {
        var collection = new CustomCollection<int>();
        var result = collection.GetReverseEnumerator().ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void GenerateSequence_ReturnsCorrectSequence()
    {
        var sequence = CustomCollection<int>.GenerateSequence(5, 3).ToList();

        Assert.Equal(new[] { 5, 6, 7 }, sequence);
    }

    [Fact]
    public void GenerateSequence_ZeroCount_ReturnsEmpty()
    {
        var sequence = CustomCollection<int>.GenerateSequence(10, 0).ToList();

        Assert.Empty(sequence);
    }

    [Fact]
    public void GenerateSequence_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CustomCollection<int>.GenerateSequence(0, -1).ToList());
    }

    [Fact]
    public void FilterAndSort_ReturnsFilteredAndSortedItems()
    {
        var collection = new CustomCollection<int>();
        collection.Add(3);
        collection.Add(1);
        collection.Add(2);
        collection.Add(5);

        var result = collection.FilterAndSort(x => x > 1, x => x).ToList();

        Assert.Equal(new[] { 2, 3, 5 }, result);
    }

    [Fact]
    public void FilterAndSort_WithStrings_FiltersAndSortsCorrectly()
    {
        var collection = new CustomCollection<string>();
        collection.Add("banana");
        collection.Add("apple");
        collection.Add("cherry");
        collection.Add("avocado");

        var result = collection
            .FilterAndSort(x => x.StartsWith("a"), x => x)
            .ToList();

        Assert.Equal(new[] { "apple", "avocado" }, result);
    }

    [Fact]
    public void Remove_RemovesElementFromCollection()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);

        var removed = collection.Remove(2);

        Assert.True(removed);
        Assert.Single(collection);
        Assert.DoesNotContain(2, collection);
    }
}
