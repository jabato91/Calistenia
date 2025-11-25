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
        private readonly DbService? _dbService; //acceso a la base de datos
        private readonly ExerciseFilterViewModel? _filter; //filtro de ejercicios
        private readonly Exercise? _selectedExercise; //ejercicio seleccionado

        private readonly ModeEnum _mode; //modo de la página (ver, editar, crear, filtrar)




        private async void editImage(object sender, EventArgs e) //método para editar la imagen del ejercicio
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions //abre el explorador de archivos para seleccionar una imagen
                {
                    PickerTitle = "Selecciona una imagen",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null && sender is ImageButton btn) //verifica que se haya seleccionado una imagen y que el remitente sea un ImageButton
                {
                    string nombreArchivo = IOPath.GetFileName(result.FullPath); //obtiene el nombre del archivo seleccionado
                    string carpetaImagenes = IOPath.Combine(FileSystem.AppDataDirectory, "Images"); //ruta de la carpeta donde se guardarán las imágenes

                    if (!Directory.Exists(carpetaImagenes)) //verifica si la carpeta no existe
                        Directory.CreateDirectory(carpetaImagenes); //crea la carpeta

                    string rutaDestino = IOPath.Combine(carpetaImagenes, nombreArchivo); //ruta completa del archivo destino
                    File.Copy(result.FullPath, rutaDestino, true); //copia el archivo seleccionado a la carpeta destino

                    btn.Source = ImageSource.FromFile(rutaDestino); //actualiza la fuente de la imagen del botón
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
        private void BuildUI() // Construye la interfaz de ejercicios según el modo
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
        
        private void BuildViewUI() //construye la interfaz de visualización del ejercicio
        {
            BackgroundColor = Color.FromArgb("#80000000");

            var titleLabel = new Label //título del ejercicio
            {
                Text = _selectedExercise.name,
                FontSize = 26,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.FromArgb("#C49362"),
                FontFamily = "EatMeAlive"
            };

            var descriptionLabel = new Label //descripción del ejercicio
            {
                Text = _selectedExercise.description,
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.White,
                FontFamily = "ComfortaaBold",
                Margin = new Thickness(10, 0)
            };

            var materialsLabel = new Label//materiales necesarios para el ejercicio
            {
                Text = _selectedExercise.materials,
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.YellowGreen,
                FontFamily = "ComfortaaBold",
                Margin = new Thickness(10, 0)
            };

           

            var videoElement = new MediaElement //elemento de video para mostrar el video del ejercicio
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
        private void BuildFilterExerciseUI() //filtra los ejercicios según los criterios seleccionados
        {
            
            var filterBodyPartEntry = new Picker //filtra por tipo de cuerpo
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

            var filterDificultyEntry = new Picker //filtra por dificultad
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
            var filterNameEntry = new Entry //filtra por nombre de rutina
            {
                Placeholder = "Nombre de la rutina",
                PlaceholderColor = Color.FromArgb("#C49362"),
                Keyboard = Keyboard.Text,
                TextColor = Color.FromArgb("#C49362"),
                Margin = 1,
                FontFamily = "ComfortaaBold",
                BackgroundColor = Color.FromArgb("#3B2523"),
            };

            var filterButton = new Button //botón para aplicar los filtros seleccionados
            {
                Text = "Filtrar",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                Command = new Command(async () => //cuando se presiona el botón
                {
                    
                    _filter!.NameRoutineFilter =
                    string.IsNullOrWhiteSpace(filterNameEntry.Text)
                    ? null
                    : filterNameEntry.Text; // actualiza los datos del nombre de la rutina


                    var selectedDiff = enumExtension.DifficultyTranslations.FirstOrDefault(x => x.Value == (string)filterDificultyEntry.SelectedItem).Key; // obtiene la dificultad seleccionada
                    _filter.DificultyFilter = selectedDiff; //actualiza el filtro de dificultad

                    var selectedBody = enumExtension.BodyTranslations.FirstOrDefault(x => x.Value == (string)filterBodyPartEntry.SelectedItem).Key; // obtiene la parte del cuerpo seleccionada
                    _filter.BodyPartFilter = selectedBody; //actualiza el filtro de parte del cuerpo


                    await Navigation.PopModalAsync(); //cierra la página modal
                })
            };

            var cancelButton = new Button
            {
                Text = "Cancelar",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                Command = new Command(async () => await Navigation.PopModalAsync()) //comando que cierra la página modal
            }; //botón para cancelar y cerrar la página modal

            // UI FINAL
            var modalPage = new ContentPage //crea la página modal para filtrar ejercicios
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
                    new Label //titulo de la página modal
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

            Content = modalPage.Content; //muestra la página modal
            BackgroundColor = modalPage.BackgroundColor; //muestra el fondo de la página modal
        }
        private void ModifyOrCreateExercise(Exercise exercise = null) //modifica o crea un ejercicio
        {
            bool newExercise = false;
            if (exercise == null) //verifica si el ejercicio es nulo
            {
                newExercise = true; //indica que es un nuevo ejercicio
                exercise = new Exercise //crea un nuevo ejercicio con valores predeterminados
                {
                    name = "",
                    description = "",
                    muscleGroupId = bodyPartEnum.nothing,
                    dificulty = dificultyEnum.nothing
                };
            }

            var nameEntry = new Entry//entrada para el nombre del ejercicio
            {
                Text = exercise.name,
                Placeholder = "Nombre",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var descEntry = new Entry //entrada para la descripción del ejercicio
            {
                Text = exercise.description,
                Placeholder = "Descripción",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var imageButton = new ImageButton //botón para seleccionar la imagen del ejercicio
            {

                WidthRequest = 75,
                HeightRequest = 75,
                BackgroundColor = Color.FromArgb("#3B2523")
            };

            imageButton.Clicked += async (s, e) => //evento al hacer clic en el botón de imagen
            {
                try
                {
                    var result = await FilePicker.Default.PickAsync(new PickOptions//abre el explorador de archivos para seleccionar una imagen
                    {
                        PickerTitle = "Selecciona una imagen",
                        FileTypes = FilePickerFileType.Images
                    });

                    if (result != null) //verifica que se haya seleccionado una imagen
                    {
                        string nombreArchivo = IOPath.GetFileName(result.FullPath); //obtiene el nombre del archivo seleccionado
                        string carpetaImagenes = IOPath.Combine(FileSystem.AppDataDirectory, "Images"); //ruta de la carpeta donde se guardarán las imágenes
                        Directory.CreateDirectory(carpetaImagenes); //crea la carpeta si no existe

                        string rutaDestino = IOPath.Combine(carpetaImagenes, nombreArchivo); //ruta completa del archivo destino

                        using var origen = await result.OpenReadAsync(); //abre el archivo seleccionado para lectura
                        using var destino = File.Create(rutaDestino); //crea el archivo destino
                        await origen.CopyToAsync(destino); //copia el contenido del archivo seleccionado al archivo destino

                        imageButton.BindingContext = rutaDestino; // guardamos la ruta
                        imageButton.Source = ImageSource.FromFile(rutaDestino); //actualiza la fuente de la imagen del botón
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            };
            Label videoLabel = new Label //etiqueta para el botón de video
            {
                Text = "Añadir video",
                TextColor = Color.FromArgb("#C49362"),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontFamily = "ComfortaaBold"
            };

            var videoFrame = new Frame //marco para el botón de video
            {
                CornerRadius = 10,
                BackgroundColor = Color.FromArgb("#3B2523"),
                HeightRequest = 100,
                WidthRequest = 150,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = videoLabel
            };

            string selectedVideoName = null; //variable para almacenar el nombre del video seleccionado

            var videoTap = new TapGestureRecognizer();//gesto para detectar el toque en el marco de video
            videoTap.Tapped += async (s, e) => //evento al tocar el marco de video
            {
                var result = await FilePicker.PickAsync(new PickOptions //abre el explorador de archivos para seleccionar un video
                {
                    PickerTitle = "Seleccionar video",
                    FileTypes = FilePickerFileType.Videos
                });

                if (result != null) //verifica que se haya seleccionado un video
                {
                    string folder = IOPath.Combine(FileSystem.AppDataDirectory, "Videos"); //ruta de la carpeta donde se guardarán los videos
                    Directory.CreateDirectory(folder); //crea la carpeta si no existe

                    string destPath = IOPath.Combine(folder, result.FileName); //ruta completa del archivo destino

                    using var src = await result.OpenReadAsync(); //abre el archivo seleccionado para lectura
                    using var dest = File.Create(destPath); //crea el archivo destino
                    await src.CopyToAsync(dest); //copia el contenido del archivo seleccionado al archivo destino

                    selectedVideoName = result.FileName; //almacena el nombre del video seleccionado
                    videoLabel.Text = "Cambiar video"; //actualiza el texto de la etiqueta del botón de video

                    await DisplayAlert("Video añadido", "El video se ha guardado correctamente.", "OK"); //muestra una alerta indicando que el video se ha guardado correctamente
                }
            };

            videoFrame.GestureRecognizers.Add(videoTap); //agrega el gesto de toque al marco de video

            var bodyPartEnumPicker = new Picker //selector para el tipo de cuerpo
            {
                Title = "Tipo Cuerpo",
                TitleColor = Color.FromArgb("#C49362"),
                ItemsSource = enumExtension.BodyTranslations.Values.ToList(),
                SelectedItem = enumExtension.BodyTranslations[exercise.muscleGroupId],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var dificultyEnumPicker = new Picker //selector para el tipo de dificultad
            {
                Title = "Tipo de dificultad",
                TitleColor = Color.FromArgb("#C49362"),
                ItemsSource = enumExtension.DifficultyTranslations.Values.ToList(),
                SelectedItem = enumExtension.DifficultyTranslations[exercise.dificulty],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var modalPage = new ContentPage //crea la página modal para modificar o crear un ejercicio
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
                            new Button //botón para crear o modificar el ejercicio
                            {
                                Text = "Crear",
                                BackgroundColor = Color.FromArgb("ffd700"),
                                TextColor = Colors.White,
                                CornerRadius = 3,
                                Command = new Command(async () =>
                                {
                                    try
                                    {

                                        if (string.IsNullOrWhiteSpace(nameEntry.Text)) //verifica que se haya ingresado un nombre
                                        {
                                            await DisplayAlert("Aviso", "Debes ingresar un nombre.", "OK");
                                            return;
                                        }
                                        if(newExercise){ //si es un nuevo ejercicio
                                            var createExercise = new Exercise //crea un nuevo ejercicio con los datos ingresados
                                            {
                                                name = nameEntry.Text,
                                                description = descEntry.Text,
                                                muscleGroupId = enumExtension.BodyTranslations.First(x => x.Value == bodyPartEnumPicker.SelectedItem.ToString()).Key,
                                                dificulty = enumExtension.DifficultyTranslations.First(x => x.Value == dificultyEnumPicker.SelectedItem.ToString()).Key,
                                                typeUser = userTypeEnum.admin,
                                                video = selectedVideoName //guarda el nombre del video seleccionado
                                            };

                                            // Guardar imagen
                                            if (imageButton.BindingContext is string rutaImg && File.Exists(rutaImg)) //verifica que se haya seleccionado una imagen
                                            {
                                                string fileName = IOPath.GetFileName(rutaImg); //obtiene el nombre del archivo de la imagen
                                                createExercise.image = fileName;
                                            }

                                            await _dbService.Create(createExercise); //crea el ejercicio en la base de datos
                                            _filter.Exercises.Add(createExercise); //agrega el nuevo ejercicio a la lista de ejercicios filtrados
                                            await DisplayAlert("Éxito", "Ejercicio creado correctamente", "OK");
                                        }
                                        else //si es una modificación de un ejercicio existente
                                        {
                                            exercise.name = nameEntry.Text ?? ""; //actualiza los datos del ejercicio con los datos ingresados
                                            exercise.description = descEntry.Text ?? ""; 
                                            exercise.dificulty = enumExtension.DifficultyTranslations.First(x => x.Value == dificultyEnumPicker.SelectedItem.ToString()).Key;
                                            exercise.muscleGroupId = enumExtension.BodyTranslations.First(x => x.Value == bodyPartEnumPicker.SelectedItem.ToString()).Key;
                                            
                                            if (imageButton.BindingContext is string rutaNueva && File.Exists(rutaNueva)) //verifica que se haya seleccionado una nueva imagen
                                            {
                                                string nombreArchivo = IOPath.GetFileName(rutaNueva);
                                                exercise.image = nombreArchivo;
                                            }

                                            await _dbService.Update(exercise); //actualiza el ejercicio en la base de datos

                                            var index = _filter.Exercises.IndexOf(_selectedExercise); //busca el índice del ejercicio modificado en la lista de ejercicios filtrados
                                            if (index >= 0) //si se encuentra el ejercicio
                                            {
                                                var ex = _filter.Exercises[index]; //obtiene el ejercicio de la lista y actualiza sus datos

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
        private MediaSource GetVideoSource(string videoName) //método para obtener la fuente del video
        {
            if (string.IsNullOrWhiteSpace(videoName)) //verifica si el nombre del video es nulo o vacío
                return null; //devuelve nulo si no hay video

            
            string path = IOPath.Combine(FileSystem.AppDataDirectory, "Videos", videoName); //optiene la ruta completa del video en la carpeta de videos

            if (File.Exists(path)) //verifica si el archivo de video existe en la ruta especificada
            {
                return MediaSource.FromFile(path); //devuelve la fuente del video desde el archivo
            }

            
            try
            {
                var rawSource = MediaSource.FromResource(videoName); //intenta obtener la fuente del video desde los recursos incrustados
                return rawSource; //devuelve la fuente del video desde los recursos
            }
            catch
            {
                Console.WriteLine("[VIDEO ERROR] No existe en RAW → " + videoName);
            }

            Console.WriteLine("[VIDEO ERROR] No existe el video → " + videoName);
            return null; //si no se encuentra el video, devuelve nulo
        }
        
    }

}

