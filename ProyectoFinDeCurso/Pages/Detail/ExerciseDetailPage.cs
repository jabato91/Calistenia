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
        private readonly DbService? _dbService;
        private readonly ExerciseFilterViewModel? _filter;
        private readonly Exercise? _selectedExercise;

        private readonly ModeEnum _mode;


       

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



        public ExerciseDetailPage(DbService? dbService = null,ExerciseFilterViewModel? filter = null,Exercise? exercise = null, ModeEnum mode = ModeEnum.nothing, userTypeEnum typeUser = userTypeEnum.nothing)
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
                case ModeEnum.create:
                    ModifyOrCreateExercise();
                    break;

                case ModeEnum.Edit:
                    ModifyOrCreateExercise(_selectedExercise);
                    break;

                case ModeEnum.View:
                    BuildViewUI();
                    break;
                case ModeEnum.filter:
                    BuildFilterExerciseUI();
                    break;
                
            }
        }
        
        private void BuildViewUI()
        {
            BackgroundColor = Color.FromArgb("#80000000");

            var titleLabel = new Label
            {
                Text = _selectedExercise.name,
                FontSize = 26,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.FromArgb("#C49362"),
                FontFamily = "EatMeAlive"
            };

            var descriptionLabel = new Label
            {
                Text = _selectedExercise.description,
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.White,
                FontFamily = "ComfortaaBold",
                Margin = new Thickness(10, 0)
            };

            var materialsLabel = new Label
            {
                Text = _selectedExercise.materials,
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.YellowGreen,
                FontFamily = "ComfortaaBold",
                Margin = new Thickness(10, 0)
            };

           

            var videoElement = new MediaElement
            {
                Source = GetVideoSource(_selectedExercise.video),
                Aspect = Aspect.AspectFit,
                ShouldShowPlaybackControls = true,
                HeightRequest = 325,
                WidthRequest = 500
            };

            var videoFrame = new Frame
            {
                CornerRadius = 15,
                HasShadow = true,
                BackgroundColor = Colors.Black,
                Padding = 0,
                Margin = new Thickness(0, 10),
                Content = videoElement
            };


            var exitButton = new Button
            {
                Text = "Salir",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                FontFamily = "ComfortaaBold",
                FontSize = 16,
                Padding = new Thickness(10, 6),
                HorizontalOptions = LayoutOptions.Fill,
                Command = new Command(async () => await Navigation.PopModalAsync())
            };

            Content = new Grid
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
    {
        new Border
        {
            BackgroundColor = Color.FromArgb("#251A18"),
                                    StrokeShape = new RoundRectangle { CornerRadius = 25 },
                                    Stroke = Colors.Orange,
                                    StrokeThickness = 2,
                                    Padding = 20,
            Content = new VerticalStackLayout
            {
                Spacing = 15,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    titleLabel,
                    descriptionLabel,
                    materialsLabel,
                    videoFrame,
                    exitButton
                }
            }
        }
    }
            };
        }
        private void BuildFilterExerciseUI()
        {
            // PICKER: BODY PART
            var filterBodyPartEntry = new Picker
            {
                Title = "Tipo Cuerpo",
                TitleColor = Color.FromArgb("#C49362"),
                ItemsSource = enumExtension.BodyTranslations.Values.ToList(),
                SelectedItem = enumExtension.BodyTranslations[bodyPartEnum.nothing],
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
                ItemsSource = enumExtension.DifficultyTranslations.Values.ToList(),
                SelectedItem = enumExtension.DifficultyTranslations[dificultyEnum.nothing],
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
                    var selectedDiff = enumExtension.DifficultyTranslations.FirstOrDefault(x => x.Value == (string)filterDificultyEntry.SelectedItem).Key;
                    _filter.DificultyFilter = selectedDiff;

                    // 🔥 3. Actualizar filtro de parte del cuerpo
                    var selectedBody = enumExtension.BodyTranslations.FirstOrDefault(x => x.Value == (string)filterBodyPartEntry.SelectedItem).Key;
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
                        Text = "Buscar Ejercicio",
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
        private void ModifyOrCreateExercise(Exercise exercise = null)
        {
            bool newExercise = false;
            if (exercise == null)
            {
                newExercise = true;
                exercise = new Exercise
                {
                    name = "",
                    description = "",
                    muscleGroupId = bodyPartEnum.nothing,
                    dificulty = dificultyEnum.nothing
                };
            }

            // ENTRADAS
            var nameEntry = new Entry
            {
                Text = exercise.name,
                Placeholder = "Nombre",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var descEntry = new Entry
            {
                Text = exercise.description,
                Placeholder = "Descripción",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var imageButton = new ImageButton
            {

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
                        string nombreArchivo = IOPath.GetFileName(result.FullPath);
                        string carpetaImagenes = IOPath.Combine(FileSystem.AppDataDirectory, "Images");
                        Directory.CreateDirectory(carpetaImagenes);

                        string rutaDestino = IOPath.Combine(carpetaImagenes, nombreArchivo);

                        using var origen = await result.OpenReadAsync();
                        using var destino = File.Create(rutaDestino);
                        await origen.CopyToAsync(destino);

                        imageButton.BindingContext = rutaDestino;
                        imageButton.Source = ImageSource.FromFile(rutaDestino);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            };
            Label videoLabel = new Label
            {
                Text = "Añadir video",
                TextColor = Color.FromArgb("#C49362"),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontFamily = "ComfortaaBold"
            };

            var videoFrame = new Frame
            {
                CornerRadius = 10,
                BackgroundColor = Color.FromArgb("#3B2523"),
                HeightRequest = 100,
                WidthRequest = 150,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = videoLabel
            };

            string selectedVideoName = null;

            var videoTap = new TapGestureRecognizer();
            videoTap.Tapped += async (s, e) =>
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Seleccionar video",
                    FileTypes = FilePickerFileType.Videos
                });

                if (result != null)
                {
                    string folder = IOPath.Combine(FileSystem.AppDataDirectory, "Videos");
                    Directory.CreateDirectory(folder);

                    string destPath = IOPath.Combine(folder, result.FileName);

                    using var src = await result.OpenReadAsync();
                    using var dest = File.Create(destPath);
                    await src.CopyToAsync(dest);

                    selectedVideoName = result.FileName;
                    videoLabel.Text = "Cambiar video";

                    await DisplayAlert("Video añadido", "El video se ha guardado correctamente.", "OK");
                }
            };

            videoFrame.GestureRecognizers.Add(videoTap);

            // PICKERS DE ENUMS
            var bodyPartEnumPicker = new Picker
            {
                Title = "Tipo Cuerpo",
                TitleColor = Color.FromArgb("#C49362"),
                ItemsSource = enumExtension.BodyTranslations.Values.ToList(),
                SelectedItem = enumExtension.BodyTranslations[exercise.muscleGroupId],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var dificultyEnumPicker = new Picker
            {
                Title = "Tipo de dificultad",
                TitleColor = Color.FromArgb("#C49362"),
                ItemsSource = enumExtension.DifficultyTranslations.Values.ToList(),
                SelectedItem = enumExtension.DifficultyTranslations[exercise.dificulty],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
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
                    MaximumWidthRequest = 500,
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
                        TextColor = Color.FromArgb("#C49362"),
                        HorizontalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily = "EatMeAlive"
                    },
                    nameEntry,
                    descEntry,
                    imageButton,
                    videoFrame,
                    bodyPartEnumPicker,
                    dificultyEnumPicker,

                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Button
                            {
                                Text = "Crear",
                                BackgroundColor = Color.FromArgb("ffd700"),
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
                                        if(newExercise){
                                            var createExercise = new Exercise
                                            {
                                                name = nameEntry.Text,
                                                description = descEntry.Text,
                                                muscleGroupId = enumExtension.BodyTranslations.First(x => x.Value == bodyPartEnumPicker.SelectedItem.ToString()).Key,
                                                dificulty = enumExtension.DifficultyTranslations.First(x => x.Value == dificultyEnumPicker.SelectedItem.ToString()).Key,
                                                typeUser = userTypeEnum.admin,
                                                video = selectedVideoName // <--- Guardamos solo el nombre del archivo
                                            };

                                            // Guardar imagen
                                            if (imageButton.BindingContext is string rutaImg && File.Exists(rutaImg))
                                            {
                                                string fileName = IOPath.GetFileName(rutaImg);
                                                createExercise.image = fileName;
                                            }

                                            await _dbService.Create(createExercise);
                                            _filter.Exercises.Add(createExercise);
                                            await DisplayAlert("Éxito", "Ejercicio creado correctamente", "OK");
                                        }
                                        else
                                        {
                                            exercise.name = nameEntry.Text ?? "";
                                            exercise.description = descEntry.Text ?? "";
                                            exercise.dificulty = enumExtension.DifficultyTranslations.First(x => x.Value == dificultyEnumPicker.SelectedItem.ToString()).Key;
                                            exercise.muscleGroupId = enumExtension.BodyTranslations.First(x => x.Value == bodyPartEnumPicker.SelectedItem.ToString()).Key;
                                            
                                            if (imageButton.BindingContext is string rutaNueva && File.Exists(rutaNueva))
                                            {
                                                string nombreArchivo = IOPath.GetFileName(rutaNueva);
                                                exercise.image = nombreArchivo;
                                            }

                                            await _dbService.Update(exercise);

                                            var index = _filter.Exercises.IndexOf(_selectedExercise);
                                            if (index >= 0)
                                            {
                                                var ex = _filter.Exercises[index];

                                                ex.name = exercise.name;
                                                ex.description = exercise.description;
                                                ex.image = exercise.image;
                                                ex.muscleGroupId = exercise.muscleGroupId;
                                                ex.dificulty = exercise.dificulty;
                                                ex.video = exercise.video;
                                            }

                                            await DisplayAlert("Éxito", "Ejercicio actualizado correctamente", "OK");
                                        }
                                        _filter.UpdateFilteredExercises();

                                        await Navigation.PopModalAsync();

                                    }
                                    catch (Exception ex)
                                    {
                                        await DisplayAlert("Error", ex.Message, "OK");
                                    }
                                })
                            },
                            new Button
                            {
                                Text = "Cancelar",
                                BackgroundColor = Color.FromArgb("ffd700"),
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
        private MediaSource GetVideoSource(string videoName)
        {
            if (string.IsNullOrWhiteSpace(videoName))
                return null;

            // 1️⃣ APPDATA → prioridad absoluta
            string path = IOPath.Combine(FileSystem.AppDataDirectory, "Videos", videoName);

            if (File.Exists(path))
            {
                Console.WriteLine("[VIDEO] Cargando desde APPDATA → " + path);
                return MediaSource.FromFile(path);
            }

            // 2️⃣ RAW → si no está en AppData
            try
            {
                var rawSource = MediaSource.FromResource(videoName);
                Console.WriteLine("[VIDEO] Cargando desde RAW → " + videoName);
                return rawSource;
            }
            catch
            {
                Console.WriteLine("[VIDEO ERROR] No existe en RAW → " + videoName);
            }

            Console.WriteLine("[VIDEO ERROR] No existe el video → " + videoName);
            return null;
        }
        
    }

}

