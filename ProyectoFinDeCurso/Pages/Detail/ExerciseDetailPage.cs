using CommunityToolkit.Maui.Views;
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
        public ExerciseDetailPage(Exercise exercise)
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
                                    Text = exercise.name,
                                    FontSize = 26,
                                    HorizontalOptions = LayoutOptions.Center,
                                    TextColor = Color.FromArgb("#C49362"),
                                    FontFamily = "EatMeAlive"
                                },
                                new Label
                                {
                                    Text = exercise.description,
                                    FontSize = 16,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    TextColor = Colors.White,
                                    FontFamily = "ComfortaaBold",
                                    Margin = new Thickness(10, 0)
                                },
                                new Label
                                {
                                    Text = exercise.materials,
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
                                        Source = MediaSource.FromResource(exercise.video),
                                        Aspect = Aspect.AspectFit,
                                        ShouldShowPlaybackControls = true,
                                        HeightRequest = 325,
                                        WidthRequest = 500
                                    }
                                },
                                new Button
                                {
                                    Text = "Salir",
                                    TextColor = Color.FromArgb("#C49362"),
                                    BackgroundColor = Color.FromArgb("#3B2523"),
                                    CornerRadius = 10,
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

        public ExerciseDetailPage(DbService dbService, ExerciseFilterViewModel filter, Exercise selectedExercise)
        {
            
            _dbService = dbService;
            _filter = filter;
            _selectedExercise = selectedExercise;

            // Cuando la página aparezca, carga los datos del ejercicio
            Appearing += ExerciseDetailPage_Appearing;
        }
        public ExerciseDetailPage(DbService dbService, ExerciseFilterViewModel filter)
        {
            _dbService = dbService;
            _filter = filter;

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

            // Botón para elegir imagen
            var imageButton = new ImageButton
            {
                HorizontalOptions = LayoutOptions.Fill,
                WidthRequest = 75,
                HeightRequest = 75
            };

            imageButton.Clicked += async (s, e) =>
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona una imagen",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    imageButton.Source = ImageSource.FromFile(result.FullPath);

                    // ✅ Guardamos la ruta completa en el BindingContext
                    imageButton.BindingContext = result.FullPath;
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

            // Frame principal de la página
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
                                    Command = new Command(async () =>
                                    {
                                        try
                                        {
                                            // Crear objeto Exercise
                                            Exercise createExercise = new Exercise();

                                            var selectedMuscleTranslation = bodyPartEnumPicker.SelectedItem?.ToString();
                                            var selectedMuscleEnum = transtation.FirstOrDefault(x => x.Value == selectedMuscleTranslation).Key;
                                            var selectedDificultyTranslation = dificultyEnumPicker.SelectedItem?.ToString();
                                            var selectedDificultyEnum = dificulty.FirstOrDefault(x => x.Value == selectedDificultyTranslation).Key;

                                            createExercise.name = nameEntry.Text ?? "";
                                            createExercise.description = descEntry.Text ?? "";
                                            createExercise.muscleGroupId = selectedMuscleEnum;
                                            createExercise.dificulty = selectedDificultyEnum;
                                            createExercise.typeUser = userTypeEnum.admin;

                                            // ✅ Copiar la imagen (si existe)
                                            if (imageButton.BindingContext is string rutaOrigen && File.Exists(rutaOrigen))
                                            {
                                                string nombreArchivo = IOPath.GetFileName(rutaOrigen);
                                                string carpetaImagenes = IOPath.Combine(FileSystem.AppDataDirectory, "Images");
                                                if (!Directory.Exists(carpetaImagenes))
                                                    Directory.CreateDirectory(carpetaImagenes);

                                                string rutaDestino = IOPath.Combine(carpetaImagenes, nombreArchivo);

                                                if (!File.Exists(rutaDestino) || rutaOrigen != rutaDestino)
                                                    File.Copy(rutaOrigen, rutaDestino, overwrite: true);

                                                createExercise.image = nombreArchivo;
                                            }
                                            else
                                            {
                                                await DisplayAlert("Aviso", "No se seleccionó una imagen válida.", "OK");
                                            }

                                            // Guardar en la base de datos
                                            await _dbService.Create(createExercise);

                                            // Añadir a la lista y refrescar
                                            _filter.Exercises.Add(createExercise);
                                            _filter.OnPropertyChanged(nameof(_filter.FilteredExercises));

                                            await DisplayAlert("Éxito", "Ejercicio creado correctamente", "OK");
                                            await Navigation.PopModalAsync();
                                            _filter.LoadExercises();
                                        }
                                        catch (Exception ex)
                                        {
                                            await DisplayAlert("Error", $"Ocurrió un error al crear el ejercicio:\n{ex.Message}", "OK");
                                        }
                                    })
                                },

                                // Botón Cancelar
                                new Button
                                {
                                    Text = "Cancelar",
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
        private async void editImage(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona una imagen",
                    FileTypes = FilePickerFileType.Images // Puedes poner .Pdf, .Videos, etc.
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir el archivo: {ex.Message}", "OK");
            }

        }
        private async void ExerciseDetailPage_Appearing(object sender, EventArgs e)
        {
            await LoadExerciseDetailAsync();
           
        }

        private async Task LoadExerciseDetailAsync()
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
                HorizontalOptions = LayoutOptions.Fill
            };

            var descEntry = new Entry
            {
                Text = exerciseFromDb.description,
                Placeholder = "Descripción",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var imageButton = new ImageButton
            {
                Source = exerciseFromDb.image,
                HorizontalOptions = LayoutOptions.Fill,
                WidthRequest = 75,
                HeightRequest = 75
            };

            imageButton.Clicked += async (s, e) =>
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona una imagen",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    imageButton.Source = ImageSource.FromFile(result.FullPath);
                }
            };

            var bodyPartEnumPicker = new Picker
            {
                Title = "Tipo Cuerpo",
                ItemsSource = traducciones.Values.ToList(),
                SelectedItem = traducciones[exerciseFromDb.muscleGroupId],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            // Creamos la interfaz (idéntica a la tuya)
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
                            FontFamily = "EatMeAlive"
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
                                    Command = new Command(async () =>
                                    {
                                        exerciseFromDb.name = nameEntry.Text ?? "";
                                        exerciseFromDb.description = descEntry.Text ?? "";

                                        if (imageButton.Source is FileImageSource fileSource)
                                        {
                                            string rutaOrigen = fileSource.File;
                                            string nombreArchivo = IOPath.GetFileName(rutaOrigen);
                                            string carpetaImagenes = IOPath.Combine(FileSystem.AppDataDirectory, "Images");

                                            if (!Directory.Exists(carpetaImagenes))
                                                Directory.CreateDirectory(carpetaImagenes);

                                            string rutaDestino = IOPath.Combine(carpetaImagenes, nombreArchivo);

                                            if (!File.Exists(rutaDestino) || rutaOrigen != rutaDestino)
                                            {
                                                try
                                                {
                                                    File.Copy(rutaOrigen, rutaDestino, overwrite: true);
                                                }
                                                catch (Exception ex)
                                                {
                                                    await DisplayAlert("Error", $"No se pudo copiar la imagen: {ex.Message}", "OK");
                                                }
                                            }

                                            exerciseFromDb.image = nombreArchivo;
                                        }

                                        await _dbService.Update(exerciseFromDb);

                                        var index = _filter.Exercises.IndexOf(_selectedExercise);
                                        if (index >= 0)
                                        {
                                            _filter.Exercises[index] = exerciseFromDb;
                                            _filter.OnPropertyChanged(nameof(_filter.FilteredExercises));
                                        }

                                        await Navigation.PopModalAsync();
                                    })
                                },
                                new Button
                                {
                                    Text = "Cancelar",
                                    Command = new Command(async () => await Navigation.PopModalAsync())
                                }
                            }
                        }
                    }
                    }
                }
            };

            // 👇 Aquí la clave:
            // usamos el contenido de modalPage directamente en esta página
            Content = modalPage.Content;
            BackgroundColor = modalPage.BackgroundColor;
        }

        private void LoadCreateExercise()
        {
           

        }
    }
}
    
