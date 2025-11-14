using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using IOPath = System.IO.Path;

namespace ProyectoFinDeCurso.Pages.Detail
{
    public class ExerciseDetailPage : ContentPage
    {
        private readonly DbService _dbService;
        private readonly ExerciseFilterViewModel _filter;
        private readonly Exercise _selectedExercise;

        private readonly ExerciseMode _mode;


       

        private async void editImage(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona una imagen",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null && sender is ImageButton btn)
                {
                    // ✅ Copiamos la imagen a la carpeta local segura
                    string nombreArchivo = IOPath.GetFileName(result.FullPath);
                    string carpetaImagenes = IOPath.Combine(FileSystem.AppDataDirectory, "Images");

                    if (!Directory.Exists(carpetaImagenes))
                        Directory.CreateDirectory(carpetaImagenes);

                    string rutaDestino = IOPath.Combine(carpetaImagenes, nombreArchivo);
                    File.Copy(result.FullPath, rutaDestino, true);

                    // ✅ Mostramos la imagen desde la carpeta interna
                    btn.Source = ImageSource.FromFile(rutaDestino);
                    btn.BindingContext = rutaDestino; // guardamos la ruta
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir el archivo: {ex.Message}", "OK");
            }
        }



        public ExerciseDetailPage(DbService? dbService = null,ExerciseFilterViewModel? filter = null,Exercise? exercise = null,ExerciseMode mode = ExerciseMode.nothing, userTypeEnum typeUser = userTypeEnum.nothing)
        {
            _dbService = dbService;
            _filter = filter;
            _selectedExercise = exercise;
            _mode = mode;

            BuildUI();
        }
        private void BuildUI()
        {
            switch (_mode)
            {
                case ExerciseMode.create:
                    BuildCreateUI();
                    break;

                case ExerciseMode.Edit:
                    BuildEditUI();
                    break;

                case ExerciseMode.View:
                    BuildViewUI();
                    break;
                case ExerciseMode.filter:
                    BuildFilterExerciseUI();
                    break;
            }
        }

        private void BuildViewUI()
        {
            BackgroundColor = Color.FromArgb("#80000000");

            Content = new Grid
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Frame
                    {
                        BackgroundColor = Color.FromArgb("#2E1E1B"),
                        CornerRadius = 20,
                        Padding = new Thickness(20, 25),
                        HasShadow = true,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 15,
                            HorizontalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new Label
                                {
                                    Text = _selectedExercise.name,
                                    FontSize = 26,
                                    HorizontalOptions = LayoutOptions.Center,
                                    TextColor = Color.FromArgb("#C49362"),
                                    FontFamily = "EatMeAlive"
                                },
                                new Label
                                {
                                    Text = _selectedExercise.description,
                                    FontSize = 16,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    TextColor = Colors.White,
                                    FontFamily = "ComfortaaBold",
                                    Margin = new Thickness(10, 0)
                                },
                                new Label
                                {
                                    Text = _selectedExercise.materials,
                                    FontSize = 16,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    TextColor = Colors.YellowGreen,
                                    FontFamily = "ComfortaaBold",
                                    Margin = new Thickness(10, 0)
                                },
                                new Frame
                                {
                                    CornerRadius = 15,
                                    HasShadow = true,
                                    BackgroundColor = Colors.Black,
                                    Padding = 0,
                                    Margin = new Thickness(0, 10, 0, 10),
                                    Content = new MediaElement
                                    {
                                        Source = MediaSource.FromResource(_selectedExercise.video),
                                        Aspect = Aspect.AspectFit,
                                        ShouldShowPlaybackControls = true,
                                        HeightRequest = 325,
                                        WidthRequest = 500
                                    }
                                },
                                new Button
                                {
                                    Text = "Salir",
                                    BackgroundColor =  Color.FromArgb("ffd700"),
                                TextColor = Colors.White,
                                CornerRadius = 3,
                                    FontFamily = "ComfortaaBold",
                                    FontSize = 16,
                                    Padding = new Thickness(10, 6),
                                    HorizontalOptions = LayoutOptions.Fill,
                                    Command = new Command(async () => await Navigation.PopModalAsync())
                                }
                            }
                        }
                    }
                }
            };
        }

        private async void BuildEditUI()
        {
            Exercise exerciseFromDb = await _dbService.GetExerciseById(_selectedExercise.execiseID);

            var traducciones = new Dictionary<bodyPartEnum, string>
    {
        { bodyPartEnum.nothing, "Ninguno" },
        { bodyPartEnum.chest, "Pecho" },
        { bodyPartEnum.leg, "Piernas" },
        { bodyPartEnum.triceps, "Tríceps" },
        { bodyPartEnum.biceps, "Bíceps" },
        { bodyPartEnum.abdomen, "Abdomen" },
        { bodyPartEnum.back, "Espalda" },
        { bodyPartEnum.shoulder, "Hombros" },
        { bodyPartEnum.isometric, "Isométrico" },
        { bodyPartEnum.arms, "Brazos" },
        { bodyPartEnum.torso, "Torso" },
        { bodyPartEnum.torsoAndArms, "Torso y Brazos" }
    };

            var nameEntry = new Entry
            {
                
                Text = exerciseFromDb.name,
                Placeholder = "Nombre",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill,
                FontFamily = "ComfortaaBold",
            };

            var descEntry = new Entry
            {
                Text = exerciseFromDb.description,
                Placeholder = "Descripción",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill,
                FontFamily = "ComfortaaBold",
            };

            // ✅ Cargar imagen local si existe
            string rutaImagen = IOPath.Combine(FileSystem.AppDataDirectory, "Images", exerciseFromDb.image ?? "");
            var imageButton = new ImageButton
            {
                Source = File.Exists(rutaImagen)
                    ? ImageSource.FromFile(rutaImagen)
                    : null,
                HorizontalOptions = LayoutOptions.Fill,
                WidthRequest = 75,
                HeightRequest = 75,
                BackgroundColor = Color.FromArgb("#3B2523")
            };

            // ✅ Vinculamos con el método editImage corregido
            imageButton.Clicked += editImage;

            var bodyPartEnumPicker = new Picker
            {
                Title = "Tipo Cuerpo",
                TitleColor = Color.FromArgb("#C49362"),
                ItemsSource = traducciones.Values.ToList(),
                SelectedItem = traducciones[exerciseFromDb.muscleGroupId],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill,
                FontFamily = "ComfortaaBold",
                
            };

            var modalPage = new ContentPage
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                Content = new Frame
                {
                    BackgroundColor = Color.FromArgb("#2E1E1B"),
                    CornerRadius = 20,
                    Margin = 1,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Content = new VerticalStackLayout
                    {
                        Padding = 1,
                        Spacing = 5,
                        Children =
                {
                    new Label
                    {
                        Text = "Modificar ejercicio",
                        FontSize = 24,
                        TextColor = Color.FromArgb("#C77B30"),
                        HorizontalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily = "EatMeAlive",
                        
                    },
                    nameEntry,
                    descEntry,
                    imageButton,
                    bodyPartEnumPicker,
                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Button
                            {
                                Text = "Guardar",
                                BackgroundColor =  Color.FromArgb("ffd700"),
                                TextColor = Colors.White,
                                CornerRadius = 3,
                                Command = new Command(async () =>
                                {
                                    exerciseFromDb.name = nameEntry.Text ?? "";
                                    exerciseFromDb.description = descEntry.Text ?? "";

                                    // ✅ Si el usuario cambió la imagen
                                    if (imageButton.BindingContext is string rutaNueva && File.Exists(rutaNueva))
                                    {
                                        string nombreArchivo = IOPath.GetFileName(rutaNueva);
                                        exerciseFromDb.image = nombreArchivo;
                                    }

                                    await _dbService.Update(exerciseFromDb);

                                    var index = _filter.Exercises.IndexOf(_selectedExercise);
                                    if (index >= 0)
                                    {
                                        _filter.Exercises[index] = exerciseFromDb;
                                        _filter.OnPropertyChanged(nameof(_filter.FilteredExercises));
                                    }

                                    await DisplayAlert("Éxito", "Ejercicio actualizado correctamente", "OK");
                                    await Navigation.PopModalAsync();
                                })
                            },
                            new Button
                            {
                                Margin = new Thickness(10,0,0,0),
                                Text = "Cancelar",
                                BackgroundColor =  Color.FromArgb("ffd700"),
                                TextColor = Colors.White,
                                CornerRadius = 3,
                                Command = new Command(async () => await Navigation.PopModalAsync())
                            }
                        }
                    }
                }
                    }
                }
            };

            Content = modalPage.Content;
            BackgroundColor = modalPage.BackgroundColor;
        }

        private void BuildCreateUI()
        {

            var transtation = new Dictionary<bodyPartEnum, string>
{
    { bodyPartEnum.nothing, "Ninguno" },
    { bodyPartEnum.chest, "Pecho" },
    { bodyPartEnum.leg, "Piernas" },
    { bodyPartEnum.triceps, "Tríceps" },
    { bodyPartEnum.biceps, "Bíceps" },
    { bodyPartEnum.abdomen, "Abdomen" },
    { bodyPartEnum.back, "Espalda" },
    { bodyPartEnum.shoulder, "Hombros" },
    { bodyPartEnum.isometric, "Isométrico" },
    { bodyPartEnum.arms, "Brazos" },
    { bodyPartEnum.torso, "Torso" },
    { bodyPartEnum.torsoAndArms, "Torso y Brazos" }
};

            var dificulty = new Dictionary<dificultyEnum, string>
{
    { dificultyEnum.nothing, "Ninguno" },
    { dificultyEnum.easy, "Fácil" },
    { dificultyEnum.medium, "Medio" },
    { dificultyEnum.hard, "Difícil" },
    { dificultyEnum.extreme, "Muy Difícil" }
};

            // Entradas de texto
            var nameEntry = new Entry
            {
                Placeholder = "Nombre",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var descEntry = new Entry
            {
                Placeholder = "Descripción",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            // Botón para elegir imagen (✅ corregido)
            var imageButton = new ImageButton
            {
                HorizontalOptions = LayoutOptions.Fill,
                WidthRequest = 75,
                HeightRequest = 75,
                BackgroundColor = Color.FromArgb("#3B2523")
            };

            imageButton.Clicked += async (s, e) =>
            {
                try
                {
                    var result = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Selecciona una imagen",
                        FileTypes = FilePickerFileType.Images
                    });

                    if (result != null)
                    {
                        // ✅ Carpeta local segura
                        string nombreArchivo = IOPath.GetFileName(result.FullPath);
                        string carpetaImagenes = IOPath.Combine(FileSystem.AppDataDirectory, "Images");

                        if (!Directory.Exists(carpetaImagenes))
                            Directory.CreateDirectory(carpetaImagenes);

                        string rutaDestino = IOPath.Combine(carpetaImagenes, nombreArchivo);

                        // ✅ Copiar usando stream (no bloquea el archivo)
                        using (var origen = await result.OpenReadAsync())
                        using (var destino = File.Create(rutaDestino))
                        {
                            await origen.CopyToAsync(destino);
                        }

                        // ✅ Mostrar imagen desde la copia local
                        imageButton.BindingContext = rutaDestino;
                        imageButton.Source = ImageSource.FromFile(rutaDestino);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo cargar la imagen: {ex.Message}", "OK");
                }
            };

            // Picker de grupo muscular
            var bodyPartEnumPicker = new Picker
            {
                Title = "Tipo Cuerpo",
                ItemsSource = transtation.Values.ToList(),
                SelectedItem = transtation[bodyPartEnum.nothing],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            // Picker de dificultad
            var dificultyEnumPicker = new Picker
            {
                Title = "Tipo de dificultad",
                ItemsSource = dificulty.Values.ToList(),
                SelectedItem = dificulty[dificultyEnum.nothing],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            // Frame principal de la página (mantengo todo tu diseño)
            var modalPage = new ContentPage
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                Content = new Frame
                {
                    BackgroundColor = Color.FromArgb("#2E1E1B"),
                    CornerRadius = 20,
                    Margin = 1,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Content = new VerticalStackLayout
                    {
                        Padding = 1,
                        Spacing = 5,
                        Children =
                {
                    new Label
                    {
                        Text = "Crear ejercicio",
                        FontSize = 24,
                        TextColor = Color.FromArgb("#C77B30"),
                        HorizontalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily = "EatMeAlive"
                    },
                    nameEntry,
                    descEntry,
                    imageButton,
                    bodyPartEnumPicker,
                    dificultyEnumPicker,
                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            // Botón Crear
                            new Button
{
    Text = "Crear",
    BackgroundColor =  Color.FromArgb("ffd700"),
                                TextColor = Colors.White,
                                CornerRadius = 3,
    Command = new Command(async () =>
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nameEntry.Text))
            {
                await DisplayAlert("Aviso", "Debes ingresar un nombre.", "OK");
                return;
            }

            Exercise createExercise = new Exercise
            {
                name = nameEntry.Text,
                description = descEntry.Text,
                muscleGroupId = transtation.First(x => x.Value == bodyPartEnumPicker.SelectedItem.ToString()).Key,
                dificulty = dificulty.First(x => x.Value == dificultyEnumPicker.SelectedItem.ToString()).Key,
                typeUser = userTypeEnum.admin
            };

            if (imageButton.BindingContext is string rutaImagen && File.Exists(rutaImagen))
{
    string nombreArchivo = IOPath.GetFileName(rutaImagen);
    string carpetaImagenes = IOPath.Combine(FileSystem.AppDataDirectory, "Images");

    if (!Directory.Exists(carpetaImagenes))
        Directory.CreateDirectory(carpetaImagenes);

    string rutaDestino = IOPath.Combine(carpetaImagenes, nombreArchivo);

    // Si ya está en la carpeta local, no se vuelve a copiar
    if (!rutaImagen.Equals(rutaDestino, StringComparison.OrdinalIgnoreCase))
    {
        try
        {
            File.Copy(rutaImagen, rutaDestino, true);
        }
        catch (IOException)
        {
            // El archivo ya está en uso o existe, ignoramos para no duplicar
        }
    }

    createExercise.image = nombreArchivo;
}
else
{
    await DisplayAlert("Aviso", "No se seleccionó una imagen válida.", "OK");
    return;
}

            // ✅ Guarda solo una vez
            await _dbService.Create(createExercise);
            _filter.Exercises.Add(createExercise);
            _filter.OnPropertyChanged(nameof(_filter.FilteredExercises));

            await DisplayAlert("Éxito", "Ejercicio creado correctamente.", "OK");
            await Navigation.PopModalAsync();
            _filter.LoadExercises();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    })
},

                            // Botón Cancelar
                            new Button
                            {
                                Text = "Cancelar",
                                BackgroundColor =  Color.FromArgb("ffd700"),
                                TextColor = Colors.White,
                                CornerRadius = 3,
                                Command = new Command(async () => await Navigation.PopModalAsync())
                            }
                        }
                    }
                }
                    }
                }
            };

            // Mostrar el diseño
            Content = modalPage.Content;
            BackgroundColor = modalPage.BackgroundColor;
        }

        private void BuildFilterExerciseUI()
        {
            var translationBodyPart = new Dictionary<bodyPartEnum, string>
    {
        { bodyPartEnum.nothing, "Ninguno" },
        { bodyPartEnum.chest, "Pecho" },
        { bodyPartEnum.leg, "Piernas" },
        { bodyPartEnum.triceps, "Tríceps" },
        { bodyPartEnum.biceps, "Bíceps" },
        { bodyPartEnum.abdomen, "Abdomen" },
        { bodyPartEnum.back, "Espalda" },
        { bodyPartEnum.shoulder, "Hombros" },
        { bodyPartEnum.isometric, "Isométrico" },
        { bodyPartEnum.arms, "Brazos" },
        { bodyPartEnum.torso, "Torso" },
        { bodyPartEnum.torsoAndArms, "Torso y Brazos" }
    };

            var translationDificulty = new Dictionary<dificultyEnum, string>
    {
        { dificultyEnum.nothing, "Ninguno" },
        { dificultyEnum.easy, "Fácil" },
        { dificultyEnum.medium, "Medio" },
        { dificultyEnum.hard, "Difícil" },
        { dificultyEnum.extreme, "Extremo" }
    };

            // PICKER: BODY PART
            var filterBodyPartEntry = new Picker
            {
                Title = "Tipo Cuerpo",
                TitleColor = Color.FromArgb("#C49362"),
                ItemsSource = translationBodyPart.Values.ToList(),
                SelectedItem = translationBodyPart[bodyPartEnum.nothing],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill,
                Margin = 1,
                FontFamily = "ComfortaaBold"
            };

            // PICKER: DIFFICULTY
            var filterDificultyEntry = new Picker
            {
                Title = "Tipo de dificultad",
                TitleColor = Color.FromArgb("#C49362"),
                ItemsSource = translationDificulty.Values.ToList(),
                SelectedItem = translationDificulty[dificultyEnum.nothing],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill,
                Margin = 1,
                FontFamily = "ComfortaaBold"
            };

            // ENTRY: NAME
            var filterNameEntry = new Entry
            {
                Placeholder = "Nombre de la rutina",
                PlaceholderColor = Color.FromArgb("#C49362"),
                Keyboard = Keyboard.Text,
                TextColor = Color.FromArgb("#C49362"),
                Margin = 1,
                FontFamily = "ComfortaaBold",
                BackgroundColor = Color.FromArgb("#3B2523"),
            };

            // BOTÓN FILTRAR — ACTUALIZA EL VIEWMODEL
            var filterButton = new Button
            {
                Text = "Filtrar",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                Command = new Command(async () =>
                {
                    // 🔥 1. Actualizar filtro de nombre
                    _filter!.NameRoutineFilter =
                    string.IsNullOrWhiteSpace(filterNameEntry.Text)
                    ? null
                    : filterNameEntry.Text;

                    // 🔥 2. Actualizar filtro de dificultad
                    var selectedDiff = translationDificulty.FirstOrDefault(x => x.Value == (string)filterDificultyEntry.SelectedItem).Key;
                    _filter.DificultyFilter = selectedDiff;

                    // 🔥 3. Actualizar filtro de parte del cuerpo
                    var selectedBody = translationBodyPart.FirstOrDefault(x => x.Value == (string)filterBodyPartEntry.SelectedItem).Key;
                    _filter.BodyPartFilter = selectedBody;


                    // 🔥 5. Cerrar modal
                    await Navigation.PopModalAsync();
                })
            };

            var cancelButton = new Button
            {
                Text = "Cancelar",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                Command = new Command(async () => await Navigation.PopModalAsync())
            };

            // UI FINAL
            var modalPage = new ContentPage
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                Content = new Border
                {
                    BackgroundColor = Color.FromArgb("#2E1E1B"),
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Margin = 1,
                    Padding = 3,
                    WidthRequest = 350,     
                    HeightRequest = 350,
                    Content = new VerticalStackLayout
                    {
                        Padding = 1,
                        Spacing = 5,
                        Children =
                {
                    new Label
                    {
                        Text = "Buscar Rutina",
                        FontSize = 24,
                        TextColor = Color.FromArgb("#C77B30"),
                        HorizontalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily = "EatMeAlive",
                        Margin= 20
                    },
                    filterNameEntry,
                    filterDificultyEntry,
                    filterBodyPartEntry,

                    new HorizontalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = 10,
                        Children =
                        {
                            filterButton,
                            cancelButton
                        }
                    }
                }
                    }
                }
            };

            Content = modalPage.Content;
            BackgroundColor = modalPage.BackgroundColor;
        }
    }

}

