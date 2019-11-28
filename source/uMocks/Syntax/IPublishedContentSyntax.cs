using System;
using Umbraco.Core.Models;
using uMocks.Builders.Abstract;

namespace uMocks.Syntax
{
  public interface IPublishedContentSyntax
  {
    IPublishedContentSyntax WithProperty<T>(string propertyAlias, T value);

    IPublishedContentSyntax WithChildren(params IPublishedContent[] children);

    IPublishedContentSyntax WithChildren(Func<IPublishedContentMockBuilder, IPublishedContentSyntax[]> childrenSyntaxFunc);

    IPublishedContentSyntax OfParent(IPublishedContent parent);

    IPublishedContentSyntax CreatedAt(DateTime createDate);

    IPublishedContentSyntax UpdatedAt(DateTime updateDate);

    IPublishedContentSyntax WithUrl(string url);

    IPublishedContent Build();
  }
}
