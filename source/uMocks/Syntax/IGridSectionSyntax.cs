namespace uMocks.Syntax
{
  public interface IGridSectionSyntax
  {
    IGridRowSyntax AddRow(string rowLayoutName);

    IGridEditorContentSyntax SubmitLayout();
  }
}
