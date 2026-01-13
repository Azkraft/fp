using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TagCloudLibrary.Preprocessor;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum PartOfSpeech
{
	[EnumMember(Value = "A")]
	Adjective,
	[EnumMember(Value = "ADV")]
	Adverb,
	[EnumMember(Value = "ADVPRO")]
	PronominalAdverb,
	[EnumMember(Value = "ANUM")]
	NumeralAdjective,
	[EnumMember(Value = "APRO")]
	AdjectivePronoun,
	[EnumMember(Value = "COM")]
	Composite,
	[EnumMember(Value = "CONJ")]
	Conjunction,
	[EnumMember(Value = "INTJ")]
	Interjection,
	[EnumMember(Value = "NUM")]
	Numeral,
	[EnumMember(Value = "PART")]
	Particle,
	[EnumMember(Value = "PR")]
	Pretext,
	[EnumMember(Value = "S")]
	Noun,
	[EnumMember(Value = "SPRO")]
	PronounNoun,
	[EnumMember(Value = "V")]
	Verb
}
