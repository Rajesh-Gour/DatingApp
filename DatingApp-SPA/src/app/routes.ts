
import { Route, RouterModule } from '@angular/router';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';


export const appRoutes: Route[] = [

    { path: '', component: HomeComponent},
    {
        path: '' ,
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [

            { path: 'members', component: MemberListComponent },
            { path: 'messages', component: MessagesComponent},
            { path: 'lists', component: ListsComponent}
        ]
    },
    
    { path: '**', redirectTo: '', pathMatch: 'full'}

];
