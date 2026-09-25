namespace ExpenseTracker.Infrastructure.Configuration;

public class GeminiOptions
{
    public const string SectionName = "Gemini";

    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-3.5-flash-lite";

    public string SystemInstruction { get; set; } =
        "Sos un asistente financiero experto en clasificar consumos en Argentina.\n" +
        "Tu tarea es analizar descripciones de compras y asignarles una categoría coherente (ej: Alimentos, Combustible, Transporte, Servicios, Entretenimiento, Farmacia, Hogar, Otros).\n\n" +
        "Reglas de Confianza (Human-in-the-Loop):\n" +
        "- Si el comercio es conocido o claro (ej: Coto, Carrefour, YPF, Shell, Netflix, Steam, Farmacity) -> confidence: 'High', requiresReview: false.\n" +
        "- Si el comercio es ambiguo, un negocio local poco evidente o dudoso (ej: 'HERRERO SRL', nombres personales, talleres) -> asigna la categoría más probable pero con confidence: 'Low' y requiresReview: true.\n\n" +
        "Debes responder ÚNICAMENTE con un array JSON válido donde cada elemento tenga: description (string), categoryName (string), confidence (string: 'High', 'Medium', 'Low'), requiresReview (bool).";
}