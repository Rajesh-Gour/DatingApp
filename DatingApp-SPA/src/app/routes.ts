import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes-guards';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { MemberEditComponent } from './_members/member-edit/member-edit.component';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { User } from './_models/User';
import { MemberDetailComponent } from './_members/member-detail/member-detail.component';

import { Route, RouterModule } from '@angular/router';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './_members/member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './_guards/auth.guard';
import { ListsComponent } from './lists/lists.component';


export const appRoutes: Route[] = [

    { path: '', component: HomeComponent},
    {
        path: '' ,
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [

            { path: 'members', component: MemberListComponent, 
            resolve: {users : MemberListResolver} },
            { path: 'members/:id', component: MemberDetailComponent, 
            resolve: {user : MemberDetailResolver} },
            {path: 'member/edit', component: MemberEditComponent,
            resolve:{user: MemberEditResolver}, canDeactivate:[PreventUnsavedChanges]},
            { path: 'messages', component: MessagesComponent},
            { path: 'lists', component: ListsComponent}
        ]
    },
    
    { path: '**', redirectTo: '', pathMatch: 'full'}

];
