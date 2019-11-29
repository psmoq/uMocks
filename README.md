# uMocks

**uMocks** is a very simple Umbraco mock object builder written using fluent design approach. It is built on top of **Moq**, **Json.Fluently** and **Umbraco.Core** libraries. **uMocks** makes Umbraco unit testing easier and more human-readable giving developers ability to fake whole document within complex values stored inside of it.

# Usage

## PublishedContentMockSession and IPublishedContentMockBuilder

You can create complex relations between mocked documents which are always being updated dynamically. Example document structure:

        [TestMethod]
        public void PublishedContent_ShouldUpdateFamilyTreeDynamically()
        {
            // Arrange

            var mockSession = PublishedContentMockSession.CreateNew(); // create new mock session
            var doc1 = mockSession.PublishedContentBuilder // user IPublishedContentMockBuilder to build your content fluently
                .PrepareNew("documentTypeAlias", documentId: 1001)
                .WithProperty("propAlias", "propValue")
                .WithChildren(stx => new[] {stx.PrepareNew("anotherDocumentTypeAlias", documentId: 1002)})
                .Build();

            // Assert

            Assert.AreEqual(1, doc1.Children.Count());
            Assert.AreEqual(1002, doc1.Children.First().Id);

            // Arrange

            var doc2 = mockSession.PublishedContentBuilder
                .PrepareNew("documentTypeAlias", documentId: 1003)
                .OfParent(doc1)
                .Build();

            // Assert

            Assert.AreEqual(2, doc1.Children.Count());
            Assert.AreEqual(2, doc1.Children.OfTypes("documentTypeAlias", "anotherDocumentTypeAlias").Count());
        }

## IGridEditorBuilder

Umbraco 7 provides powerful content editor called **[Grid Layout](https://our.umbraco.com/documentation/getting-started/backoffice/property-editors/built-in-property-editors/grid-layout/)** which allows to define rich content structures very easily. Grid Layout editor stores underlying model in complex JSON structure so it's often very inconvinient to write unit tests for grid layout related logic. **uMocks** provides fluent Grid Layout builder which gives you maximum freedom of grid content definition. You can inject it into property value without a hassle then. Together with **IPublishedContentMockBuilder** you will be able to unit test whatever service you have. Example layout definition with custom component inside:

        [TestMethod]
        public void GridComponent_ShouldHaveGivenValue()
        {
            // Arrange

            var mockSession = PublishedContentMockSession.CreateNew();
            var gridEditor = mockSession.GridEditorBuilder
                .CreateNew("1 column layout")
                .AddSection(12)
                .AddFullWidthRow()
                .SubmitLayout()
                .PutGridComponent(sectionIndex: 0, rowIndex: 0, columnIndex: 0, alias: "componentAlias", b => b.CreateNew()
                .WithProperty("propertyName1", "propertyValue1")
                .WithProperty("propertyName2", "propertyValue2"))
                .Build();

            var controlTokens = gridEditor.SelectTokens("$..controls[*]").ToArray();

            // Assert

            // grid editor should have one component defined
            Assert.AreEqual(1, controlTokens.Count()); 

            var control = controlTokens.First();

            // grid editor should have given alias
            Assert.AreEqual("componentAlias", control.SelectToken("$.editor").Value<string>("alias"));

            // grid editor should have given property value
            Assert.AreEqual("propertyValue1", control.SelectToken("$.value.value").Value<string>("propertyName1"));
        }

# Samples

For further exploration, please check out uMocks.Samples project

# TODO in near future

There is a small road map of uMocks feature development:

  * Examine search result mocking helper implementation
  * IContent mock implementation
  * Umbraco ContentService events sandbox implementation

# License

MIT License

Copyright (c) 2019 psmoq

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
