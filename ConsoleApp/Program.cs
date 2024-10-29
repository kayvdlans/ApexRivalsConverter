// Temporary while I'm working on my mac.

var input = Path.Combine(Environment.CurrentDirectory, "test_input", "inputTest.xml");
var dir = Path.Combine(Environment.CurrentDirectory, "test_output");
new AMS2ToApexRivals.Converter(input, dir).Convert();