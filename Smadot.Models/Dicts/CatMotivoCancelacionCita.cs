namespace Smadot.Models.Dicts
{
	public class CatMotivoCancelacionCita
	{
		public const int CanceladoPorElUsuario = 1;
		public const int ReduccionDeHorario = 2;
		public const int DiaDeshabilitado = 3;
		public const int CambioDeIntervaloEntreCitas = 4;
		public const int ReduccionDeCapacidad = 5;

		public static Dictionary<int, string> Nombres = new()
		{
			{1, "Cancelado por el usuario" },
			{2, "Reducción de horario" },
			{3, "Día deshabilitado" },
			{4, "Cambio de intervalo entre citas" },
			{5, "Reducción de capacidad" }
		};

	}
}
