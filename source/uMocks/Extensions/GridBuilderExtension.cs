using uMocks.Syntax;

namespace uMocks.Extensions
{
  public static class GridBuilderExtension
  {
    public static IGridSectionSyntax AddFullWidthRow(this IGridSectionSyntax stx)
    {
      return stx.AddRow("full width").WithColumns(1);
    }

    public static IGridSectionSyntax AddHalvedRow(this IGridSectionSyntax stx)
    {
      return stx.AddRow("6-6").WithColumns(2);
    }

    public static IGridSectionSyntax AddOneThirdRow(this IGridSectionSyntax stx)
    {
      return stx.AddRow("4-4-4").WithColumns(3);
    }

    public static IGridSectionSyntax AddQuarteredRow(this IGridSectionSyntax stx)
    {
      return stx.AddRow("3-3-3-3").WithColumns(4);
    }

    public static IGridSectionSyntax AddFourToEightRow(this IGridSectionSyntax stx)
    {
      return stx.AddRow("4-8").WithColumns(2);
    }

    public static IGridSectionSyntax AddEightToFourRow(this IGridSectionSyntax stx)
    {
      return stx.AddRow("8-4").WithColumns(2);
    }
  }
}
