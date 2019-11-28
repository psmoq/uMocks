using System;
using Json.Fluently.Builders.Abstract;
using Newtonsoft.Json.Linq;

namespace uMocks.Syntax
{
  public interface IGridEditorContentSyntax : IGridEditorSyntax
  {
    IGridEditorContentSyntax PutControl(int sectionIndex, int rowIndex, int columnIndex, string alias, JObject value);

    IGridEditorContentSyntax PutControl(int sectionIndex, int rowIndex, int columnIndex,
      string alias, Func<IFluentJsonBuilder, IJsonObjectBuilder> objectSyntaxFunc);
  }
}
